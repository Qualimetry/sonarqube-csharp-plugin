using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EmptyStringComparisonAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0054);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var binary = (BinaryExpressionSyntax)context.Node;

        var leftIsEmpty = IsEmptyStringLiteral(binary.Left);
        var rightIsEmpty = IsEmptyStringLiteral(binary.Right);
        if (leftIsEmpty == rightIsEmpty)
        {
            return;
        }

        var operand = leftIsEmpty ? binary.Right : binary.Left;
        if (context.SemanticModel.GetTypeInfo(operand).Type?.SpecialType != SpecialType.System_String)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0054, binary.GetLocation()));
    }

    private static bool IsEmptyStringLiteral(ExpressionSyntax expression) =>
        expression is LiteralExpressionSyntax literal
        && literal.IsKind(SyntaxKind.StringLiteralExpression)
        && literal.Token.ValueText.Length == 0;
}
