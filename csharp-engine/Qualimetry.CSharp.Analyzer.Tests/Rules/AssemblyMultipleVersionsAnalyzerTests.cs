using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AssemblyMultipleVersionsAnalyzerTests
{
    // {|qa_quality_assembly_multiple_versions:SharedLib|}

    [Fact]
    public Task SingleVersionGraph_IsClean()
    {
        const string source = """
            public sealed class App
            {
            }
            """;

        return CSharpAnalyzerVerifier<AssemblyMultipleVersionsAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public async Task MultipleVersions_ReportsDiagnostic()
    {
        const string lib = """
            namespace Lib
            {
                public sealed class Marker
                {
                }
            }
            """;

        MetadataReference versionOne = MultiAssemblyTestHelper.CompileLibrary("SharedLib", "1.0.0.0", lib, alias: "V1");
        MetadataReference versionTwo = MultiAssemblyTestHelper.CompileLibrary("SharedLib", "2.0.0.0", lib, alias: "V2");

        var references = (await ReferenceAssemblies.Net.Net80.ResolveAsync(
            LanguageNames.CSharp,
            CancellationToken.None)).ToList();
        references.Add(versionOne);
        references.Add(versionTwo);

        const string source = """
            extern alias V1;
            extern alias V2;
            namespace App
            {
                public sealed class Program
                {
                }
            }
            """;

        var compilation = CSharpCompilation.Create(
            "App",
            new[] { CSharpSyntaxTree.ParseText(source) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.DoesNotContain(errors, d => d.Id != "CS1704");

        var analyzer = new AssemblyMultipleVersionsAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer),
            new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty));

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        Assert.Contains(
            diagnostics,
            d => d.Id == "qa_quality_assembly_multiple_versions");
    }
}
