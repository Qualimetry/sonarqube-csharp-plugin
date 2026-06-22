using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ReaderWriterLockAcquireWithoutReleaseAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> ReleaseMethods = ImmutableHashSet.Create(
        "ReleaseReaderLock",
        "ReleaseWriterLock",
        "ReleaseLock");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0137);

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

        if (!IsReaderWriterLock(method.ContainingType))
        {
            return;
        }

        if (method.Name != "AcquireReaderLock" && method.Name != "AcquireWriterLock")
        {
            return;
        }

        var body = LockPairing.EnclosingBody(invocation);
        if (body is null)
        {
            return;
        }

        if (HasRelease(context, body))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0137, invocation.GetLocation()));
    }

    private static bool IsReaderWriterLock(INamedTypeSymbol? type)
    {
        return type?.Name == "ReaderWriterLock"
            && type.ContainingNamespace?.ToDisplayString() == "System.Threading";
    }

    private static bool HasRelease(SyntaxNodeAnalysisContext context, SyntaxNode body)
    {
        foreach (var invocation in body.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol method
                && ReleaseMethods.Contains(method.Name)
                && IsReaderWriterLock(method.ContainingType))
            {
                return true;
            }
        }

        return false;
    }
}
