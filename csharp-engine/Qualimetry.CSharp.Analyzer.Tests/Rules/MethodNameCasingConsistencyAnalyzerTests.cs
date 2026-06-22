using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MethodNameCasingConsistencyAnalyzerTests
{
    [Fact]
    public Task SameNameDifferentCasing_IsReported()
    {
        const string source = """
            public class Mailbox
            {
                public void Send()
                {
                }

                public void {|qa_naming_method_name_casing_consistency:send|}()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MethodNameCasingConsistencyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DistinctNames_IsClean()
    {
        const string source = """
            public class Mailbox
            {
                public void Send()
                {
                }

                public void Receive()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MethodNameCasingConsistencyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task Overloads_IsClean()
    {
        const string source = """
            public class Mailbox
            {
                public void Send()
                {
                }

                public void Send(int count)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MethodNameCasingConsistencyAnalyzer>.VerifyAsync(source);
    }
}
