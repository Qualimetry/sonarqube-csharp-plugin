using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Qualimetry.CSharp.Analyzer.Tests;

internal static class MultiAssemblyTestHelper
{
    internal static MetadataReference CompileLibrary(string assemblyName, string version, string source, string? alias = null)
    {
        string fullSource = $$"""
            using System.Reflection;
            [assembly: AssemblyVersion("{{version}}")]
            {{source}}
            """;

        var compilation = CSharpCompilation.Create(
            assemblyName,
            new[] { CSharpSyntaxTree.ParseText(fullSource) },
            ReferenceAssemblies.Net.Net80.ResolveAsync(LanguageNames.CSharp, CancellationToken.None).GetAwaiter().GetResult(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        if (!result.Success)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString())));
        }

        stream.Position = 0;
        var properties = alias == null
            ? MetadataReferenceProperties.Assembly
            : new MetadataReferenceProperties(MetadataImageKind.Assembly, aliases: ImmutableArray.Create(alias));

        return MetadataReference.CreateFromStream(stream, properties);
    }

    internal static Task VerifyWithReferencesAsync<TAnalyzer>(string source, params MetadataReference[] references)
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        var externAliases = new StringBuilder();
        foreach (MetadataReference reference in references)
        {
            if (reference is PortableExecutableReference portable
                && portable.Properties.Aliases is { Length: > 0 } aliases)
            {
                externAliases.AppendLine($"extern alias {aliases[0]};");
            }
        }

        var test = new AnalyzerTest<TAnalyzer> { TestCode = externAliases + source };
        foreach (MetadataReference reference in references)
        {
            test.TestState.AdditionalReferences.Add(reference);
        }

        return test.RunAsync(CancellationToken.None);
    }

    private sealed class AnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public AnalyzerTest()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            CompilerDiagnostics = CompilerDiagnostics.None;
        }
    }
}
