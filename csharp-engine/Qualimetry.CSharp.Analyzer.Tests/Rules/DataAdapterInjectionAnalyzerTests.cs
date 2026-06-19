using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DataAdapterInjectionAnalyzerTests
{
    private const string Stub = """
        public sealed class TestDataAdapter
        {
            public TestDataAdapter(string sql, object connection) { }
        }
        """;

    [Fact]
    public Task ConcatenatedAdapterQuery_IsReported()
    {
        string source = Stub + """

            public sealed class Reports
            {
                public object Build(object connection, string status)
                {
                    return new TestDataAdapter({|qa_reliability_data_adapter_injection:"SELECT * FROM Orders WHERE Status = '" + status + "'"|}, connection);
                }
            }
            """;

        return CSharpAnalyzerVerifier<DataAdapterInjectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConstantAdapterQuery_IsClean()
    {
        string source = Stub + """

            public sealed class Reports
            {
                public object Build(object connection, string status)
                {
                    return new TestDataAdapter("SELECT * FROM Orders WHERE Status = @status", connection);
                }
            }
            """;

        return CSharpAnalyzerVerifier<DataAdapterInjectionAnalyzer>.VerifyAsync(source);
    }
}
