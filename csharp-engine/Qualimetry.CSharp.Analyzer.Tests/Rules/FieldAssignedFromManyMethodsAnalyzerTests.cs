using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class FieldAssignedFromManyMethodsAnalyzerTests
{
    [Fact]
    public Task FieldAssignedFromManyMethods_IsReported()
    {
        const string source = """
            public class Session
            {
                private int {|qa_metrics_field_assigned_from_many_methods:_state|};

                public void Open() { _state = 1; }
                public void Pause() { _state = 2; }
                public void Close() { _state = 3; }
            }
            """;

        return CSharpAnalyzerVerifier<FieldAssignedFromManyMethodsAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_field_assigned_from_many_methods.maxassigningmethods", "2"));
    }

    [Fact]
    public Task FieldAssignedFromFewMethods_IsClean()
    {
        const string source = """
            public class Session
            {
                private int _state;

                public void Open() { _state = 1; }
                public void Close() { _state = 2; }
            }
            """;

        return CSharpAnalyzerVerifier<FieldAssignedFromManyMethodsAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_field_assigned_from_many_methods.maxassigningmethods", "2"));
    }
}
