using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BooleanConditionalSimplificationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0153);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ConditionalExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var conditional = (ConditionalExpressionSyntax)context.Node;
        if (!IsBoolLiteral(conditional.WhenTrue) || !IsBoolLiteral(conditional.WhenFalse))
        {
            return;
        }

        if (conditional.WhenTrue.Kind() == conditional.WhenFalse.Kind())
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0153, conditional.GetLocation()));
    }

    private static bool IsBoolLiteral(ExpressionSyntax expression)
    {
        return expression.IsKind(SyntaxKind.TrueLiteralExpression) || expression.IsKind(SyntaxKind.FalseLiteralExpression);
    }
}
