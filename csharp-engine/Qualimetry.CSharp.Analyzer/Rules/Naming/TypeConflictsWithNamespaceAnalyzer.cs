using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TypeConflictsWithNamespaceAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0200);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.Locations.All(location => !location.IsInSource))
        {
            return;
        }

        var containingNamespace = type.ContainingNamespace;
        if (containingNamespace == null || containingNamespace.IsGlobalNamespace)
        {
            return;
        }

        if (!string.Equals(type.Name, containingNamespace.Name, StringComparison.Ordinal))
        {
            return;
        }

        foreach (var location in type.Locations)
        {
            if (location.IsInSource)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0200,
                    location,
                    type.Name,
                    containingNamespace.ToDisplayString()));
            }
        }
    }
}
