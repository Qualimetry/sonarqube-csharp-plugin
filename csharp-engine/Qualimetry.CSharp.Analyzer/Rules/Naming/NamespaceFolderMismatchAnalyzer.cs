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
        var tree = type.Locations.FirstOrDefault(location => location.IsInSource).SourceTree;
        if (tree == null)
        {
            return;
        }

        var namespaceName = NamespacePathHelper.GetNamespaceName(type.ContainingNamespace);
        if (namespaceName == null)
        {
            return;
        }

        var directory = NamespacePathHelper.GetSourceDirectory(tree);
        if (directory == null)
        {
            return;
        }

        if (NamespacePathHelper.NamespaceMatchesDirectory(namespaceName, directory))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0203,
            type.Locations.First(location => location.IsInSource),
            namespaceName,
            directory));
    }
}
