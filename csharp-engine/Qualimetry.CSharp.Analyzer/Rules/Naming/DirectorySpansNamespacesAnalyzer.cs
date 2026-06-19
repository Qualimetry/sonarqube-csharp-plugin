using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Qualimetry.CSharp.Analyzer.Helpers;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DirectorySpansNamespacesAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0205);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var namespacesByDirectory = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>(StringComparer.OrdinalIgnoreCase);
        var typesByDirectory = new ConcurrentDictionary<string, ConcurrentBag<INamedTypeSymbol>>(StringComparer.OrdinalIgnoreCase);

        context.RegisterSymbolAction(
            symbolContext =>
            {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                var namespaceName = NamespacePathHelper.GetNamespaceName(type.ContainingNamespace);
                if (namespaceName == null)
                {
                    return;
                }

                var tree = type.Locations.FirstOrDefault(location => location.IsInSource).SourceTree;
                var directory = tree == null ? null : NamespacePathHelper.GetSourceDirectory(tree);
                if (directory == null)
                {
                    return;
                }

                namespacesByDirectory
                    .GetOrAdd(directory, _ => new ConcurrentDictionary<string, byte>(StringComparer.Ordinal))
                    [namespaceName] = 0;
                typesByDirectory.GetOrAdd(directory, _ => new ConcurrentBag<INamedTypeSymbol>()).Add(type);
            },
            SymbolKind.NamedType);

        context.RegisterCompilationEndAction(
            endContext =>
            {
                foreach (var entry in namespacesByDirectory)
                {
                    if (entry.Value.Count < 2 || !typesByDirectory.TryGetValue(entry.Key, out ConcurrentBag<INamedTypeSymbol>? types))
                    {
                        continue;
                    }

                    foreach (var type in types.Distinct(SymbolEqualityComparer.Default))
                    {
                        var location = type.Locations.FirstOrDefault(location => location.IsInSource);
                        if (location != null)
                        {
                            endContext.ReportDiagnostic(Diagnostic.Create(
                                Descriptors.QCS0205,
                                location,
                                entry.Key));
                        }
                    }
                }
            });
    }
}
