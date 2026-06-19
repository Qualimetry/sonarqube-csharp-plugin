using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DuplicateTypeAcrossAssembliesAnalyzerTests
{
    // {|qa_quality_duplicate_type_across_assemblies:Shared.Models.Customer|}

    [Fact]
    public Task SingleTypeDefinition_IsClean()
    {
        const string source = """
            namespace Models
            {
                public sealed class Customer
                {
                    public string Name { get; set; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<DuplicateTypeAcrossAssembliesAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public async Task DuplicateMetadataNameAcrossAssemblies_IsReported()
    {
        const string lib = """
            namespace Shared.Models
            {
                public sealed class Customer
                {
                    public string Name { get; set; }
                }
            }
            """;

        MetadataReference libOne = MultiAssemblyTestHelper.CompileLibrary("SharedModelsOne", "1.0.0.0", lib);
        MetadataReference libTwo = MultiAssemblyTestHelper.CompileLibrary("SharedModelsTwo", "2.0.0.0", lib);

        var references = (await ReferenceAssemblies.Net.Net80.ResolveAsync(
            LanguageNames.CSharp,
            CancellationToken.None)).ToList();
        references.Add(libOne);
        references.Add(libTwo);

        const string source = """
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

        var analyzer = new DuplicateTypeAcrossAssembliesAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer),
            new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty));

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        Assert.Contains(
            diagnostics,
            d => d.Id == "qa_quality_duplicate_type_across_assemblies");
    }
}
