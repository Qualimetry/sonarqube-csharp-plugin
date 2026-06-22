using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AssemblyMultipleVersionsAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Descriptors.QCS0209);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationAction(Analyze);
    }

    private static void Analyze(CompilationAnalysisContext context)
    {
        var versionsByName = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        void AddIdentity(AssemblyIdentity identity)
        {
            if (IsFrameworkAssemblyName(identity.Name))
            {
                return;
            }

            if (!versionsByName.TryGetValue(identity.Name, out HashSet<string>? versions))
            {
                versions = new HashSet<string>(StringComparer.Ordinal);
                versionsByName[identity.Name] = versions;
            }

            versions.Add(identity.Version.ToString());
        }

        AddIdentity(context.Compilation.Assembly.Identity);

        foreach (MetadataReference reference in context.Compilation.References)
        {
            if (context.Compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assembly)
            {
                AddIdentity(assembly.Identity);
            }
        }

        var conflictingAssemblyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Diagnostic diagnostic in context.Compilation.GetDiagnostics())
        {
            if (diagnostic.Id != "CS1704")
            {
                continue;
            }

            if (!TryParseDuplicateAssemblyName(diagnostic.GetMessage(), out string assemblyName)
                || IsFrameworkAssemblyName(assemblyName))
            {
                continue;
            }

            conflictingAssemblyNames.Add(assemblyName);

            if (!versionsByName.TryGetValue(assemblyName, out HashSet<string>? versions))
            {
                versions = new HashSet<string>(StringComparer.Ordinal);
                versionsByName[assemblyName] = versions;
            }

            versions.Add("conflicting");
        }

        foreach (KeyValuePair<string, HashSet<string>> entry in versionsByName)
        {
            if (entry.Value.Count < 2 && !conflictingAssemblyNames.Contains(entry.Key))
            {
                continue;
            }

            Location? location = context.Compilation.SyntaxTrees.FirstOrDefault()?.GetLocation(new TextSpan(0, 0));
            if (location == null)
            {
                continue;
            }

            string versions = string.Join(", ", entry.Value.OrderBy(value => value, StringComparer.Ordinal));
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0209,
                location,
                entry.Key,
                versions));
        }
    }

    private static bool TryParseDuplicateAssemblyName(string message, out string assemblyName)
    {
        assemblyName = string.Empty;
        int openQuote = message.IndexOf('\'');
        if (openQuote < 0)
        {
            return false;
        }

        int closeQuote = message.IndexOf('\'', openQuote + 1);
        if (closeQuote <= openQuote)
        {
            return false;
        }

        assemblyName = message.Substring(openQuote + 1, closeQuote - openQuote - 1);
        return assemblyName.Length > 0;
    }

    private static bool IsFrameworkAssemblyName(string name)
    {
        return name.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)
            || name.Equals("System", StringComparison.OrdinalIgnoreCase)
            || name.Equals("Microsoft", StringComparison.OrdinalIgnoreCase)
            || name.Equals("netstandard", StringComparison.OrdinalIgnoreCase)
            || name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase)
            || name.Equals("Runtime", StringComparison.OrdinalIgnoreCase);
    }
}
