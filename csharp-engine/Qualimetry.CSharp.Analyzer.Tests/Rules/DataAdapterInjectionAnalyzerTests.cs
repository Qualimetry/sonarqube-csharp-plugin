using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DataAdapterInjectionAnalyzerTests
{
    private const string FrameworkStub = """
        using System.Data.Common;

        public sealed class ReportAdapter : DataAdapter
        {
            public ReportAdapter(string sql, object connection) { }
        }
        """;

    private const string LookAlikeStub = """
        public sealed class TestDataAdapter
        {
            public TestDataAdapter(string sql, object connection) { }
        }
        """;

    [Fact]
    public Task ConcatenatedAdapterQuery_OnDataAdapter_IsReported()
    {
        string source = FrameworkStub + """

            public sealed class Reports
            {
                public object Build(object connection, string status)
                {
                    return new ReportAdapter({|qa_reliability_data_adapter_injection:"SELECT * FROM Orders WHERE Status = '" + status + "'"|}, connection);
                }
            }
            """;

        return CSharpAnalyzerVerifier<DataAdapterInjectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConstantAdapterQuery_OnDataAdapter_IsClean()
    {
        string source = FrameworkStub + """

            public sealed class Reports
            {
                private const string Status = "Open";

                public object Build(object connection)
                {
                    return new ReportAdapter("SELECT * FROM Orders WHERE Status = '" + Status + "'", connection);
                }
            }
            """;

        return CSharpAnalyzerVerifier<DataAdapterInjectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LookAlikeAdapterType_IsClean()
    {
        string source = LookAlikeStub + """

            public sealed class Reports
            {
                public object Build(object connection, string status)
                {
                    return new TestDataAdapter("SELECT * FROM Orders WHERE Status = '" + status + "'", connection);
                }
            }
            """;

        return CSharpAnalyzerVerifier<DataAdapterInjectionAnalyzer>.VerifyAsync(source);
    }
}
