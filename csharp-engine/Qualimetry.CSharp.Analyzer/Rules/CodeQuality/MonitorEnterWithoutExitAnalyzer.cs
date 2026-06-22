using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MonitorEnterWithoutExitAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0115);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (!IsMonitorCall(context, invocation, out var calledName))
        {
            return;
        }

        if (calledName is not ("Enter" or "TryEnter"))
        {
            return;
        }

        var body = LockPairing.EnclosingBody(invocation);
        if (body is null)
        {
            return;
        }

        if (LockPairing.ContainsMonitorMethod(context, body, "Exit"))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0115, invocation.GetLocation()));
    }

    private static bool IsMonitorCall(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, out string calledName)
    {
        calledName = string.Empty;

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return false;
        }

        if (method.ContainingType?.Name != "Monitor"
            || method.ContainingType.ContainingNamespace?.ToDisplayString() != "System.Threading")
        {
            return false;
        }

        calledName = method.Name;
        return true;
    }
}

internal static class LockPairing
{
    public static SyntaxNode? EnclosingBody(SyntaxNode node)
    {
        for (var current = node.Parent; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case BaseMethodDeclarationSyntax method:
                    return (SyntaxNode?)method.Body ?? method.ExpressionBody;
                case LocalFunctionStatementSyntax local:
                    return (SyntaxNode?)local.Body ?? local.ExpressionBody;
                case AccessorDeclarationSyntax accessor:
                    return (SyntaxNode?)accessor.Body ?? accessor.ExpressionBody;
            }
        }

        return null;
    }

    public static bool ContainsMonitorMethod(SyntaxNodeAnalysisContext context, SyntaxNode body, string methodName)
    {
        foreach (var invocation in body.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol method
                && method.Name == methodName
                && method.ContainingType?.Name == "Monitor"
                && method.ContainingType.ContainingNamespace?.ToDisplayString() == "System.Threading")
            {
                return true;
            }
        }

        return false;
    }
}
