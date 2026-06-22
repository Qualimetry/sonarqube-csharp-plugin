using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Qualimetry.CSharp.Analyzer.Helpers;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamespaceFolderMismatchAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0203);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var type = (INamedTypeSymbol)context.Symbol;
        if (type.ContainingType != null)
        {
            return;
        }

        var location = type.Locations.FirstOrDefault(candidate => candidate.IsInSource);
        var tree = location?.SourceTree;
        if (tree == null)
        {
            return;
        }

        var namespaceName = NamespacePathHelper.GetNamespaceName(type.ContainingNamespace);
        if (namespaceName == null)
        {
            return;
        }

        var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(tree);
        var projectDirectory = ReadProperty(options, "build_property.ProjectDir")
            ?? ReadProperty(options, "build_property.MSBuildProjectDirectory");
        var rootNamespace = ReadProperty(options, "build_property.RootNamespace");

        if (!NamespacePathHelper.TryGetNamespaceMismatch(namespaceName, tree.FilePath, projectDirectory, rootNamespace, out var expectedNamespace))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0203,
            location!,
            namespaceName,
            expectedNamespace));
    }

    private static string? ReadProperty(AnalyzerConfigOptions options, string key)
        => options.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;
}
