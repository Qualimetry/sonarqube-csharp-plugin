using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StaticRegexIsMatchAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0176);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess))
        {
            return;
        }

        if (memberAccess.Name.Identifier.ValueText != "IsMatch")
        {
            return;
        }

        if (!(context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol method) || !method.IsStatic)
        {
            return;
        }

        if (method.ContainingType?.ToDisplayString() != "System.Text.RegularExpressions.Regex")
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count < 2)
        {
            return;
        }

        if (!IsConstant(context.SemanticModel, arguments[1].Expression, context.CancellationToken))
        {
            return;
        }

        if (arguments.Count >= 3 && !IsConstant(context.SemanticModel, arguments[2].Expression, context.CancellationToken))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0176, invocation.GetLocation()));
    }

    private static bool IsConstant(SemanticModel model, ExpressionSyntax expression, System.Threading.CancellationToken cancellationToken) =>
        model.GetConstantValue(expression, cancellationToken).HasValue;
}
