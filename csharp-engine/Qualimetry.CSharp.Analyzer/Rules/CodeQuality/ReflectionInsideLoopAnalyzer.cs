using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ReflectionInsideLoopAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0084);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (!IsInsideLoop(invocation))
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return;
        }

        var containingType = method.ContainingType;
        if (containingType is null)
        {
            return;
        }

        var ns = containingType.ContainingNamespace?.ToDisplayString() ?? string.Empty;
        var isReflection = ns == "System.Reflection"
            || ns.StartsWith("System.Reflection.", System.StringComparison.Ordinal)
            || (containingType.Name == "Activator" && ns == "System" && method.Name == "CreateInstance");

        if (!isReflection)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0084, invocation.GetLocation()));
    }

    private static bool IsInsideLoop(SyntaxNode node)
    {
        for (var current = node.Parent; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case ForStatementSyntax:
                case ForEachStatementSyntax:
                case WhileStatementSyntax:
                case DoStatementSyntax:
                    return true;
                case MethodDeclarationSyntax:
                case AnonymousFunctionExpressionSyntax:
                case LocalFunctionStatementSyntax:
                    return false;
            }
        }

        return false;
    }
}
