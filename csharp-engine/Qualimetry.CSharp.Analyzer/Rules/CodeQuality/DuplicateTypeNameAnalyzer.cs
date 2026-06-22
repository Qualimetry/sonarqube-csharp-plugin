using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicateTypeNameAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0016);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var byName = new ConcurrentDictionary<string, ConcurrentBag<INamedTypeSymbol>>();

        context.RegisterSymbolAction(
            symbolContext =>
            {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                if (type.Locations.All(l => !l.IsInSource))
                {
                    return;
                }

                byName.GetOrAdd(type.Name, _ => new ConcurrentBag<INamedTypeSymbol>()).Add(type);
            },
            SymbolKind.NamedType);

        context.RegisterCompilationEndAction(
            endContext =>
            {
                foreach (var entry in byName)
                {
                    var distinct = entry.Value.Distinct(SymbolEqualityComparer.Default).ToList();
                    if (distinct.Count < 2)
                    {
                        continue;
                    }

                    foreach (var type in distinct)
                    {
                        if (type is not null)
                        {
                            ReportType(endContext, type, entry.Key);
                        }
                    }
                }
            });
    }

    private static void ReportType(CompilationAnalysisContext context, ISymbol type, string name)
    {
        foreach (var location in type.Locations)
        {
            if (location.IsInSource)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0016, location, name));
            }
        }
    }
}
