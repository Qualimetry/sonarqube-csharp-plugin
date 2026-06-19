using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CultureInsensitiveParsingAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> CultureSensitiveTypes = ImmutableHashSet.Create(
        "Double",
        "Single",
        "Decimal",
        "DateTime",
        "DateTimeOffset");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0094);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return;
        }

        if (method.Name != "Parse" && method.Name != "TryParse")
        {
            return;
        }

        var containingType = method.ContainingType;
        if (containingType is null
            || containingType.ContainingNamespace?.ToDisplayString() != "System"
            || !CultureSensitiveTypes.Contains(containingType.Name))
        {
            return;
        }

        if (method.Parameters.Any(p => p.Type.Name == "IFormatProvider"))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0094, invocation.GetLocation()));
    }
}
