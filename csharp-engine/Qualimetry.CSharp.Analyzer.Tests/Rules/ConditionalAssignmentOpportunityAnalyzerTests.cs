using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ConditionalAssignmentOpportunityAnalyzerTests
{
    [Fact]
    public Task IfElseAssigningSameVariable_IsReported()
    {
        const string source = """
            public class C
            {
                public int Clamp(bool high)
                {
                    int limit;
                    {|qa_style_conditional_assignment_opportunity:if|} (high)
                    {
                        limit = 100;
                    }
                    else
                    {
                        limit = 10;
                    }

                    return limit;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConditionalAssignmentOpportunityAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConditionalAssignment_IsClean()
    {
        const string source = """
            public class C
            {
                public int Clamp(bool high)
                {
                    int limit = high ? 100 : 10;
                    return limit;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConditionalAssignmentOpportunityAnalyzer>.VerifyAsync(source);
    }
}
