using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InterfacePrefixAnalyzerTests
{
    [Fact]
    public Task InterfaceWithoutIPrefix_IsReported()
    {
        const string source = """
            public interface {|qa_naming_interface_prefix:Repository|}
            {
                void Save();
            }
            """;

        return CSharpAnalyzerVerifier<InterfacePrefixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InterfaceWithIPrefix_IsClean()
    {
        const string source = """
            public interface IRepository
            {
                void Save();
            }
            """;

        return CSharpAnalyzerVerifier<InterfacePrefixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task Class_IsClean()
    {
        const string source = """
            public class Repository
            {
            }
            """;

        return CSharpAnalyzerVerifier<InterfacePrefixAnalyzer>.VerifyAsync(source);
    }
}
