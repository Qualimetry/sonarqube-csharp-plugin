using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Qualimetry.CSharp.Analyzer.Rules.Design;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NamespaceDependencyCycleAnalyzerTests
{
    // {|qa_architecture_namespace_dependency_cycle:BetaServices.Client|}

    [Fact]
    public async Task NamespaceCycle_IsReported()
    {
        const string source = """
namespace Alpha.Services
{
    public sealed class AlphaService
    {
        public void Run(BetaServices.Client client) => client.Execute();
    }
}

namespace BetaServices
{
    public sealed class Client
    {
        public void Execute() => new Alpha.Services.AlphaService().Run(this);
    }
}
""";

        var references = await ReferenceAssemblies.Net.Net80.ResolveAsync(
            LanguageNames.CSharp,
            CancellationToken.None);

        var compilation = CSharpCompilation.Create(
            "App",
            new[] { CSharpSyntaxTree.ParseText(source) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new NamespaceDependencyCycleAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer),
            new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty));

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        Assert.Contains(
            diagnostics,
            d => d.Id == "qa_architecture_namespace_dependency_cycle");
    }

    [Fact]
    public async Task ParentChildNamespaceReferences_AreNotACycle()
    {
        const string source = """
namespace Module
{
    public sealed class Root
    {
        public void Use(Module.Mappers.Mapper mapper) => mapper.Map();
    }
}

namespace Module.Mappers
{
    public sealed class Mapper
    {
        public void Map(Module.Utility.Helper helper = null) => helper?.Help();
    }
}

namespace Module.Utility
{
    public sealed class Helper
    {
        public void Help(Module.Root root = null) => root?.ToString();
    }
}
""";

        var references = await ReferenceAssemblies.Net.Net80.ResolveAsync(
            LanguageNames.CSharp,
            CancellationToken.None);

        var compilation = CSharpCompilation.Create(
            "App",
            new[] { CSharpSyntaxTree.ParseText(source) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new NamespaceDependencyCycleAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create<DiagnosticAnalyzer>(analyzer),
            new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty));

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
        Assert.DoesNotContain(
            diagnostics,
            d => d.Id == "qa_architecture_namespace_dependency_cycle");
    }
}
