using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Qualimetry.CSharp.Analyzer.Helpers;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamespaceSpansDirectoriesAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0204);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var directoriesByNamespace = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>(StringComparer.Ordinal);
        var typesByNamespace = new ConcurrentDictionary<string, ConcurrentBag<INamedTypeSymbol>>(StringComparer.Ordinal);

        context.RegisterSymbolAction(
            symbolContext =>
            {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                var namespaceName = NamespacePathHelper.GetNamespaceName(type.ContainingNamespace);
                if (namespaceName == null)
                {
                    return;
                }

                var tree = type.Locations.FirstOrDefault(location => location.IsInSource)?.SourceTree;
                var directory = tree == null ? null : NamespacePathHelper.GetSourceDirectory(tree);
                if (directory == null)
                {
                    return;
                }

                directoriesByNamespace
                    .GetOrAdd(namespaceName, _ => new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase))
                    [directory] = 0;
                typesByNamespace.GetOrAdd(namespaceName, _ => new ConcurrentBag<INamedTypeSymbol>()).Add(type);
            },
            SymbolKind.NamedType);

        context.RegisterCompilationEndAction(
            endContext =>
            {
                foreach (var entry in directoriesByNamespace)
                {
                    if (entry.Value.Count < 2 || !typesByNamespace.TryGetValue(entry.Key, out ConcurrentBag<INamedTypeSymbol>? types))
                    {
                        continue;
                    }

                    if (!HasUnrelatedDirectoryPair(entry.Value.Keys.ToArray()))
                    {
                        continue;
                    }

                    foreach (var type in types.Distinct(SymbolEqualityComparer.Default))
                    {
                        var location = type.Locations.FirstOrDefault(location => location.IsInSource);
                        if (location != null)
                        {
                            endContext.ReportDiagnostic(Diagnostic.Create(
                                Descriptors.QCS0204,
                                location,
                                entry.Key));
                        }
                    }
                }
            });
    }

    private static bool HasUnrelatedDirectoryPair(string[] directories)
    {
        for (int i = 0; i < directories.Length; i++)
        {
            for (int j = i + 1; j < directories.Length; j++)
            {
                if (!NamespacePathHelper.AreDirectoriesRelatedByPrefix(directories[i], directories[j]))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
