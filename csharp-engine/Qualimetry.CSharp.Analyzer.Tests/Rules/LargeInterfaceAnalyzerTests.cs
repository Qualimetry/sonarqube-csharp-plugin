using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class LargeInterfaceAnalyzerTests
{
    [Fact]
    public Task InterfaceWithManyMembers_IsReported()
    {
        const string source = """
            public interface {|qa_metrics_large_interface:IDevice|}
            {
                void Open();
                void Read();
                void Close();
            }
            """;

        return CSharpAnalyzerVerifier<LargeInterfaceAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_large_interface.maxmembers", "2"));
    }

    [Fact]
    public Task SmallInterface_IsClean()
    {
        const string source = """
            public interface IDevice
            {
                void Open();
                void Close();
            }
            """;

        return CSharpAnalyzerVerifier<LargeInterfaceAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_large_interface.maxmembers", "2"));
    }
}
