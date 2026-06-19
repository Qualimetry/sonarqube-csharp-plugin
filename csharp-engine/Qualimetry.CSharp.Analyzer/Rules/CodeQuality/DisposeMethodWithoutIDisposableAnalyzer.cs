using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DisposeMethodWithoutIDisposableAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0071);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var name = method.Identifier.ValueText;

        if (name != "Dispose" && name != "DisposeAsync")
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is not { } symbol)
        {
            return;
        }

        if (!symbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
        {
            return;
        }

        var containingType = symbol.ContainingType;
        if (containingType is null || containingType.TypeKind == TypeKind.Interface)
        {
            return;
        }

        if (ImplementsDisposable(containingType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0071, method.Identifier.GetLocation()));
    }

    private static bool ImplementsDisposable(INamedTypeSymbol type)
    {
        return type.AllInterfaces.Any(i =>
            (i.Name == "IDisposable" || i.Name == "IAsyncDisposable")
            && i.ContainingNamespace?.ToDisplayString() == "System");
    }
}
