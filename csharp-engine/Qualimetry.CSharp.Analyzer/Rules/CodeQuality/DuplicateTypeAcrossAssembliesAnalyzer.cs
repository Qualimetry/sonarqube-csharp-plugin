using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicateTypeAcrossAssembliesAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Descriptors.QCS0208);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationAction(Analyze);
    }

    private static void Analyze(CompilationAnalysisContext context)
    {
        var assembliesByMetadataName = new Dictionary<string, HashSet<IAssemblySymbol>>(StringComparer.Ordinal);

        foreach (IAssemblySymbol assembly in GetAssemblies(context.Compilation))
        {
            if (IsFrameworkAssembly(assembly))
            {
                continue;
            }

            CollectTypes(assembly.GlobalNamespace, assembly, assembliesByMetadataName);
        }

        SyntaxTree? firstTree = context.Compilation.SyntaxTrees.FirstOrDefault();
        if (firstTree == null)
        {
            return;
        }

        Location reportLocation = firstTree.GetLocation(new TextSpan(0, 0));

        foreach (KeyValuePair<string, HashSet<IAssemblySymbol>> entry in assembliesByMetadataName)
        {
            if (entry.Value.Count < 2 || ShouldIgnoreMetadataName(entry.Key))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0208,
                reportLocation,
                entry.Key));
        }
    }

    private static bool ShouldIgnoreMetadataName(string metadataName)
    {
        return metadataName.StartsWith("System.", StringComparison.Ordinal)
            || metadataName.StartsWith("Microsoft.", StringComparison.Ordinal);
    }

    private static IReadOnlyList<IAssemblySymbol> GetAssemblies(Compilation compilation)
    {
        var assemblies = new List<IAssemblySymbol> { compilation.Assembly };
        foreach (MetadataReference reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assembly
                && !assemblies.Contains(assembly, SymbolEqualityComparer.Default))
            {
                assemblies.Add(assembly);
            }
        }

        return assemblies;
    }

    private static void CollectTypes(
        INamespaceSymbol namespaceSymbol,
        IAssemblySymbol assembly,
        Dictionary<string, HashSet<IAssemblySymbol>> assembliesByMetadataName)
    {
        foreach (ISymbol member in namespaceSymbol.GetMembers())
        {
            if (member is INamespaceSymbol childNamespace)
            {
                CollectTypes(childNamespace, assembly, assembliesByMetadataName);
                continue;
            }

            if (member is not INamedTypeSymbol type
                || type.TypeKind == TypeKind.Error
                || type.IsImplicitlyDeclared
                || type.DeclaredAccessibility != Accessibility.Public
                || type.ContainingNamespace == null
                || type.ContainingNamespace.IsGlobalNamespace
                || ShouldIgnoreMetadataName(type.MetadataName))
            {
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, assembly))
            {
                continue;
            }

            string metadataName = type.MetadataName;
            if (!assembliesByMetadataName.TryGetValue(metadataName, out HashSet<IAssemblySymbol>? assemblies))
            {
                assemblies = new HashSet<IAssemblySymbol>(SymbolEqualityComparer.Default);
                assembliesByMetadataName[metadataName] = assemblies;
            }

            assemblies.Add(assembly);
        }
    }

    private static bool IsFrameworkAssembly(IAssemblySymbol assembly)
    {
        string name = assembly.Identity.Name;
        return name.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)
            || name.Equals("System", StringComparison.OrdinalIgnoreCase)
            || name.Equals("Microsoft", StringComparison.OrdinalIgnoreCase)
            || name.Equals("netstandard", StringComparison.OrdinalIgnoreCase)
            || name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase)
            || name.Equals("Runtime", StringComparison.OrdinalIgnoreCase);
    }
}
