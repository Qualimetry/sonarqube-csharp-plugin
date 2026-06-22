using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExcessiveInstanceStateAnalyzerTests
{
    [Fact]
    public Task TypeWithMuchState_IsReported()
    {
        const string source = """
            public class {|qa_metrics_excessive_instance_state:Profile|}
            {
                private int _a;
                private int _b;
                public int P { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveInstanceStateAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_instance_state.maxinstancefields", "2"));
    }

    [Fact]
    public Task TypeWithLittleState_IsClean()
    {
        const string source = """
            public class Profile
            {
                private int _a;
                public int P { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveInstanceStateAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_instance_state.maxinstancefields", "2"));
    }
}
