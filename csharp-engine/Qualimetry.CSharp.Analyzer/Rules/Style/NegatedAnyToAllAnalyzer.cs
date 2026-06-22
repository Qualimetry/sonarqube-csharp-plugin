using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NegatedAnyToAllAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0043);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.LogicalNotExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var unary = (PrefixUnaryExpressionSyntax)context.Node;
        if (LinqQueryHelper.TryGetSinglePredicateCall(context.SemanticModel, unary.Operand, "Any", out var invocation)
            && LinqQueryHelper.HasPurePredicate(invocation))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0043, invocation.GetLocation()));
        }
    }
}
