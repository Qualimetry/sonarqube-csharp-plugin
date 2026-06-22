using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class FieldCouldBeReadonlyAnalyzerTests
{
    [Fact]
    public Task FieldAssignedOnlyInConstructor_IsReported()
    {
        const string source = """
            public class Connection
            {
                private string {|qa_quality_field_could_be_readonly:_endpoint|};

                public Connection(string endpoint)
                {
                    _endpoint = endpoint;
                }

                public string Endpoint => _endpoint;
            }
            """;

        return CSharpAnalyzerVerifier<FieldCouldBeReadonlyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyField_IsClean()
    {
        const string source = """
            public class Connection
            {
                private readonly string _endpoint;

                public Connection(string endpoint)
                {
                    _endpoint = endpoint;
                }

                public string Endpoint => _endpoint;
            }
            """;

        return CSharpAnalyzerVerifier<FieldCouldBeReadonlyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task FieldMutatedInMethod_IsClean()
    {
        const string source = """
            public class Counter
            {
                private int _count;

                public Counter(int start)
                {
                    _count = start;
                }

                public void Increment()
                {
                    _count++;
                }
            }
            """;

        return CSharpAnalyzerVerifier<FieldCouldBeReadonlyAnalyzer>.VerifyAsync(source);
    }
}
