using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RedundantWhereClauseAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> PredicateOperators = ImmutableHashSet.Create(
        "Any",
        "Count",
        "First",
        "FirstOrDefault",
        "Last",
        "LastOrDefault",
        "Single",
        "SingleOrDefault");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0189);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.ArgumentList.Arguments.Count != 0
            || invocation.Expression is not MemberAccessExpressionSyntax terminal
            || !PredicateOperators.Contains(terminal.Name.Identifier.ValueText))
        {
            return;
        }

        if (terminal.Expression is not InvocationExpressionSyntax whereInvocation
            || whereInvocation.Expression is not MemberAccessExpressionSyntax whereAccess
            || whereAccess.Name.Identifier.ValueText != "Where"
            || whereInvocation.ArgumentList.Arguments.Count != 1)
        {
            return;
        }

        if (whereInvocation.ArgumentList.Arguments[0].Expression is not (SimpleLambdaExpressionSyntax or ParenthesizedLambdaExpressionSyntax))
        {
            return;
        }

        if (!IsLinqOperator(context, invocation))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0189, whereAccess.Name.GetLocation()));
    }

    private static bool IsLinqOperator(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation)
    {
        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return true;
        }

        var owner = method.ContainingType?.ToDisplayString();
        return owner is "System.Linq.Enumerable" or "System.Linq.Queryable";
    }
}
