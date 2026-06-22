using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InterfacePrefixCasingAnalyzerTests
{
    [Fact]
    public Task IPrefixFollowedByLowercase_IsReported()
    {
        const string source = """
            public interface {|qa_naming_interface_prefix_casing:Itemizer|}
            {
                void Run();
            }
            """;

        return CSharpAnalyzerVerifier<InterfacePrefixCasingAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task IPrefixFollowedByUppercase_IsClean()
    {
        const string source = """
            public interface IItemizer
            {
                void Run();
            }
            """;

        return CSharpAnalyzerVerifier<InterfacePrefixCasingAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task IPrefixFollowedByDigit_IsClean()
    {
        const string source = """
            public interface I2FactorAuth
            {
                void Run();
            }
            """;

        return CSharpAnalyzerVerifier<InterfacePrefixCasingAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InterfaceWithoutIPrefix_IsClean()
    {
        const string source = """
            public interface Repository
            {
                void Run();
            }
            """;

        return CSharpAnalyzerVerifier<InterfacePrefixCasingAnalyzer>.VerifyAsync(source);
    }
}
