using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FieldCouldBeReadonlyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0092);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
            || field.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return;
        }

        if (field.Parent is not TypeDeclarationSyntax typeDeclaration
            || typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not IFieldSymbol symbol)
            {
                continue;
            }

            if (symbol.DeclaredAccessibility != Accessibility.Private || symbol.IsVolatile)
            {
                continue;
            }

            var assignedInConstruction = variable.Initializer is not null;
            var disqualified = false;

            foreach (var reference in typeDeclaration.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                if (reference.Identifier.ValueText != symbol.Name)
                {
                    continue;
                }

                if (!SymbolEqualityComparer.Default.Equals(
                        context.SemanticModel.GetSymbolInfo(reference, context.CancellationToken).Symbol, symbol))
                {
                    continue;
                }

                if (!IsWrite(reference))
                {
                    continue;
                }

                if (IsDirectConstructorWrite(reference, symbol.IsStatic))
                {
                    assignedInConstruction = true;
                    continue;
                }

                disqualified = true;
                break;
            }

            if (!disqualified && assignedInConstruction)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0092, variable.Identifier.GetLocation()));
            }
        }
    }

    private static bool IsWrite(IdentifierNameSyntax reference)
    {
        var expression = reference.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == reference
            ? (ExpressionSyntax)memberAccess
            : reference;

        switch (expression.Parent)
        {
            case AssignmentExpressionSyntax assignment when assignment.Left == expression:
                return true;
            case PrefixUnaryExpressionSyntax prefix
                when prefix.IsKind(SyntaxKind.PreIncrementExpression) || prefix.IsKind(SyntaxKind.PreDecrementExpression):
                return true;
            case PostfixUnaryExpressionSyntax:
                return true;
            case ArgumentSyntax argument when !argument.RefOrOutKeyword.IsKind(SyntaxKind.None):
                return true;
            default:
                return false;
        }
    }

    private static bool IsDirectConstructorWrite(SyntaxNode reference, bool isStatic)
    {
        for (var current = reference.Parent; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case AnonymousFunctionExpressionSyntax:
                case LocalFunctionStatementSyntax:
                    return false;
                case ConstructorDeclarationSyntax constructor:
                    return constructor.Modifiers.Any(SyntaxKind.StaticKeyword) == isStatic;
                case MemberDeclarationSyntax:
                    return false;
            }
        }

        return false;
    }
}
