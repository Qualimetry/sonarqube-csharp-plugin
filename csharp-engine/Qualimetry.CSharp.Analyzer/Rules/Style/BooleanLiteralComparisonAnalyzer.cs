using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BooleanLiteralComparisonAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0044);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var binary = (BinaryExpressionSyntax)context.Node;

        var leftIsLiteral = IsBooleanLiteral(binary.Left);
        var rightIsLiteral = IsBooleanLiteral(binary.Right);
        if (leftIsLiteral == rightIsLiteral)
        {
            return;
        }

        var operand = leftIsLiteral ? binary.Right : binary.Left;
        if (context.SemanticModel.GetTypeInfo(operand).Type?.SpecialType != SpecialType.System_Boolean)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0044, binary.GetLocation()));
    }

    private static bool IsBooleanLiteral(ExpressionSyntax expression) =>
        expression.IsKind(SyntaxKind.TrueLiteralExpression) || expression.IsKind(SyntaxKind.FalseLiteralExpression);
}
