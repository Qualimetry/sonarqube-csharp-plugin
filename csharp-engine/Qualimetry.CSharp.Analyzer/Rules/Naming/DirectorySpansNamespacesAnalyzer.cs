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
        var projectDirectoryByDirectory = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

                namespacesByDirectory
                    .GetOrAdd(directory, _ => new ConcurrentDictionary<string, byte>(StringComparer.Ordinal))
                    [namespaceName] = 0;
                typesByDirectory.GetOrAdd(directory, _ => new ConcurrentBag<INamedTypeSymbol>()).Add(type);

                var options = symbolContext.Options.AnalyzerConfigOptionsProvider.GetOptions(tree!);
                var projectDirectory = ReadProperty(options, "build_property.ProjectDir")
                    ?? ReadProperty(options, "build_property.MSBuildProjectDirectory");
                if (projectDirectory != null)
                {
                    projectDirectoryByDirectory.TryAdd(directory, projectDirectory);
                }
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

                    if (!HasUnrelatedNamespacePair(entry.Value.Keys.ToArray()))
                    {
                        continue;
                    }

                    projectDirectoryByDirectory.TryGetValue(entry.Key, out var projectDirectory);
                    var displayDirectory = NamespacePathHelper.GetProjectRelativeDirectory(entry.Key, projectDirectory);

                    foreach (var type in types.Distinct(SymbolEqualityComparer.Default))
                    {
                        var location = type.Locations.FirstOrDefault(location => location.IsInSource);
                        if (location != null)
                        {
                            endContext.ReportDiagnostic(Diagnostic.Create(
                                Descriptors.QCS0205,
                                location,
                                displayDirectory));
                        }
                    }
                }
            });
    }

    private static bool HasUnrelatedNamespacePair(string[] namespaces)
    {
        for (int i = 0; i < namespaces.Length; i++)
        {
            for (int j = i + 1; j < namespaces.Length; j++)
            {
                if (!NamespaceGraphHelper.IsAncestorOrDescendant(namespaces[i], namespaces[j]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string? ReadProperty(AnalyzerConfigOptions options, string key)
        => options.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;
}
