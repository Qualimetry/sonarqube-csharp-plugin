using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ReservedEnumMemberNameAnalyzerTests
{
    [Fact]
    public Task ReservedEnumMember_IsReported()
    {
        const string source = """
            public enum Channel
            {
                Email,
                Sms,
                {|qa_quality_reserved_enum_member_name:Reserved|}
            }
            """;

        return CSharpAnalyzerVerifier<ReservedEnumMemberNameAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MeaningfulEnumMembers_AreClean()
    {
        const string source = """
            public enum Channel
            {
                Email,
                Sms,
                Push
            }
            """;

        return CSharpAnalyzerVerifier<ReservedEnumMemberNameAnalyzer>.VerifyAsync(source);
    }
}
