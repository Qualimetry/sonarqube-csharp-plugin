using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryReturnParenthesesAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0165);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ParenthesizedExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var parenthesized = (ParenthesizedExpressionSyntax)context.Node;
        if (!(parenthesized.Parent is ReturnStatementSyntax))
        {
            return;
        }

        if (!IsAtomic(parenthesized.Expression))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0165, parenthesized.GetLocation()));
    }

    private static bool IsAtomic(ExpressionSyntax expression)
    {
        switch (expression.Kind())
        {
            case SyntaxKind.IdentifierName:
            case SyntaxKind.NumericLiteralExpression:
            case SyntaxKind.StringLiteralExpression:
            case SyntaxKind.CharacterLiteralExpression:
            case SyntaxKind.TrueLiteralExpression:
            case SyntaxKind.FalseLiteralExpression:
            case SyntaxKind.NullLiteralExpression:
            case SyntaxKind.SimpleMemberAccessExpression:
            case SyntaxKind.InvocationExpression:
            case SyntaxKind.ElementAccessExpression:
            case SyntaxKind.ParenthesizedExpression:
                return true;
            default:
                return false;
        }
    }
}
