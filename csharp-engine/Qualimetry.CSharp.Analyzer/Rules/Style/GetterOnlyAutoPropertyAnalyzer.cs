using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class GetterOnlyAutoPropertyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0132);

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

            var read = GetGetterOnlyReadExpression(property);
            if (read is null)
            {
                continue;
            }

            if (model.GetSymbolInfo(read).Symbol is not IFieldSymbol field
                || !IsBackingFieldCandidate(field, type))
            {
                continue;
            }

            if (IsConvertible(type, property, field, read, model))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0132,
                    property.Identifier.GetLocation(),
                    property.Identifier.ValueText));
            }
        }
    }

    private static ExpressionSyntax? GetGetterOnlyReadExpression(PropertyDeclarationSyntax property)
    {
        if (property.ExpressionBody != null)
        {
            return AsFieldReference(property.ExpressionBody.Expression);
        }

        if (property.AccessorList is not { } accessors || accessors.Accessors.Count != 1)
        {
            return null;
        }

        var accessor = accessors.Accessors[0];
        if (!accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
        {
            return null;
        }

        if (accessor.ExpressionBody != null)
        {
            return AsFieldReference(accessor.ExpressionBody.Expression);
        }

        if (accessor.Body is { Statements: { Count: 1 } statements }
            && statements[0] is ReturnStatementSyntax { Expression: { } returned })
        {
            return AsFieldReference(returned);
        }

        return null;
    }

    private static ExpressionSyntax? AsFieldReference(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax identifier => identifier,
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } member => member,
            _ => null,
        };
    }

    private static bool IsBackingFieldCandidate(IFieldSymbol field, TypeDeclarationSyntax type)
    {
        if (!field.IsReadOnly || field.IsStatic || field.DeclaredAccessibility != Accessibility.Private)
        {
            return false;
        }

        if (field.DeclaringSyntaxReferences.Length != 1)
        {
            return false;
        }

        if (field.DeclaringSyntaxReferences[0].GetSyntax() is not VariableDeclaratorSyntax declarator)
        {
            return false;
        }

        if (declarator.Parent is not VariableDeclarationSyntax declaration || declaration.Variables.Count != 1)
        {
            return false;
        }

        return declaration.Parent is FieldDeclarationSyntax fieldDeclaration
            && fieldDeclaration.Parent == type;
    }

    private static bool IsConvertible(
        TypeDeclarationSyntax type,
        PropertyDeclarationSyntax property,
        IFieldSymbol field,
        ExpressionSyntax read,
        SemanticModel model)
    {
        foreach (var identifier in type.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (!SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(identifier).Symbol, field))
            {
                continue;
            }

            if (property.FullSpan.Contains(identifier.Span))
            {
                continue;
            }

            if (!IsConstructorSimpleAssignment(identifier))
            {
                return false;
            }
        }

        return read != null;
    }

    private static bool IsConstructorSimpleAssignment(IdentifierNameSyntax identifier)
    {
        ExpressionSyntax target = identifier.Parent is MemberAccessExpressionSyntax member && member.Name == identifier
            ? member
            : identifier;

        if (target.Parent is not AssignmentExpressionSyntax assignment
            || !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression)
            || assignment.Left != target)
        {
            return false;
        }

        for (SyntaxNode? current = assignment; current != null; current = current.Parent)
        {
            switch (current)
            {
                case SimpleLambdaExpressionSyntax:
                case ParenthesizedLambdaExpressionSyntax:
                case AnonymousMethodExpressionSyntax:
                case LocalFunctionStatementSyntax:
                    return false;
                case ConstructorDeclarationSyntax constructor:
                    return !constructor.Modifiers.Any(SyntaxKind.StaticKeyword);
                case BaseMethodDeclarationSyntax:
                case AccessorDeclarationSyntax:
                case BasePropertyDeclarationSyntax:
                case TypeDeclarationSyntax:
                    return false;
            }
        }

        return false;
    }
}
