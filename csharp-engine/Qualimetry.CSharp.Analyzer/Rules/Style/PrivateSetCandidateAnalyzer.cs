using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PrivateSetCandidateAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0187);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.RecordStructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var type = (TypeDeclarationSyntax)context.Node;
        if (type.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        var model = context.SemanticModel;

        foreach (var property in type.Members.OfType<PropertyDeclarationSyntax>())
        {
            if (property.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                continue;
            }

            var setAccessor = GetEligibleSetAccessor(property);
            if (setAccessor is null)
            {
                continue;
            }

            if (model.GetDeclaredSymbol(property) is not IPropertySymbol symbol)
            {
                continue;
            }

            if (IsAssignedOnlyInConstructor(type, property, symbol, model))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0187,
                    setAccessor.Keyword.GetLocation()));
            }
        }
    }

    private static AccessorDeclarationSyntax? GetEligibleSetAccessor(PropertyDeclarationSyntax property)
    {
        if (property.AccessorList is not { } accessorList)
        {
            return null;
        }

        AccessorDeclarationSyntax? getter = null;
        AccessorDeclarationSyntax? setter = null;
        foreach (var accessor in accessorList.Accessors)
        {
            if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
            {
                getter = accessor;
            }
            else if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
            {
                setter = accessor;
            }
            else
            {
                return null;
            }
        }

        if (getter is null || setter is null)
        {
            return null;
        }

        if (setter.Modifiers.Count != 0)
        {
            return null;
        }

        if (!IsAutoAccessor(getter) || !IsAutoAccessor(setter))
        {
            return null;
        }

        return setter;
    }

    private static bool IsAutoAccessor(AccessorDeclarationSyntax accessor)
    {
        return accessor.Body is null && accessor.ExpressionBody is null;
    }

    private static bool IsAssignedOnlyInConstructor(
        TypeDeclarationSyntax type,
        PropertyDeclarationSyntax property,
        IPropertySymbol symbol,
        SemanticModel model)
    {
        var constructorWrites = 0;

        foreach (var name in type.DescendantNodes().OfType<SimpleNameSyntax>())
        {
            if (name.Identifier.ValueText != symbol.Name)
            {
                continue;
            }

            if (property.FullSpan.Contains(name.Span))
            {
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(name).Symbol, symbol))
            {
                continue;
            }

            if (!IsWriteTarget(name))
            {
                continue;
            }

            if (!IsInsideInstanceConstructorOf(name, type))
            {
                return false;
            }

            constructorWrites++;
        }

        return constructorWrites > 0;
    }

    private static bool IsWriteTarget(SimpleNameSyntax name)
    {
        ExpressionSyntax target = name;
        if (name.Parent is MemberAccessExpressionSyntax member && member.Name == name)
        {
            target = member;
        }

        switch (target.Parent)
        {
            case AssignmentExpressionSyntax assignment when assignment.Left == target:
                return true;
            case PrefixUnaryExpressionSyntax prefix
                when prefix.Operand == target
                    && (prefix.IsKind(SyntaxKind.PreIncrementExpression) || prefix.IsKind(SyntaxKind.PreDecrementExpression)):
                return true;
            case PostfixUnaryExpressionSyntax postfix
                when postfix.Operand == target
                    && (postfix.IsKind(SyntaxKind.PostIncrementExpression) || postfix.IsKind(SyntaxKind.PostDecrementExpression)):
                return true;
            case ArgumentSyntax argument
                when argument.Expression == target && !argument.RefKindKeyword.IsKind(SyntaxKind.None):
                return true;
            default:
                return false;
        }
    }

    private static bool IsInsideInstanceConstructorOf(SyntaxNode node, TypeDeclarationSyntax type)
    {
        for (SyntaxNode? current = node; current != null; current = current.Parent)
        {
            switch (current)
            {
                case SimpleLambdaExpressionSyntax:
                case ParenthesizedLambdaExpressionSyntax:
                case AnonymousMethodExpressionSyntax:
                case LocalFunctionStatementSyntax:
                    return false;
                case ConstructorDeclarationSyntax constructor:
                    return !constructor.Modifiers.Any(SyntaxKind.StaticKeyword) && constructor.Parent == type;
                case BaseMethodDeclarationSyntax:
                case AccessorDeclarationSyntax:
                case TypeDeclarationSyntax:
                    return false;
            }
        }

        return false;
    }
}
