using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class GarbageCollectorCollectWithoutWaitAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0070);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (!IsGarbageCollectorCall(context, invocation, "Collect"))
        {
            return;
        }

        var body = FindEnclosingBody(invocation);
        if (body is null)
        {
            return;
        }

        foreach (var candidate in body.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (IsGarbageCollectorCall(context, candidate, "WaitForPendingFinalizers"))
            {
                return;
            }
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0070, invocation.GetLocation()));
    }

    private static SyntaxNode? FindEnclosingBody(SyntaxNode node)
    {
        for (SyntaxNode? current = node.Parent; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case MethodDeclarationSyntax method:
                    return (SyntaxNode?)method.Body ?? method.ExpressionBody;
                case AccessorDeclarationSyntax accessor:
                    return (SyntaxNode?)accessor.Body ?? accessor.ExpressionBody;
                case LocalFunctionStatementSyntax local:
                    return (SyntaxNode?)local.Body ?? local.ExpressionBody;
                case ConstructorDeclarationSyntax constructor:
                    return (SyntaxNode?)constructor.Body ?? constructor.ExpressionBody;
            }
        }

        return null;
    }

    private static bool IsGarbageCollectorCall(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, string methodName)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess
            || memberAccess.Name.Identifier.ValueText != methodName)
        {
            return false;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return false;
        }

        return method.ContainingType?.Name == "GC"
            && method.ContainingType.ContainingNamespace?.ToDisplayString() == "System";
    }
}
