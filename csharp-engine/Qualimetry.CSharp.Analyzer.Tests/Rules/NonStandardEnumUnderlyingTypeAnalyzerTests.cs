using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NonStandardEnumUnderlyingTypeAnalyzerTests
{
    [Fact]
    public Task EnumWithLongStorage_IsReported()
    {
        const string source = """
            public enum Priority : {|qa_quality_non_standard_enum_underlying_type:long|}
            {
                Low,
                Normal,
                High
            }
            """;

        return CSharpAnalyzerVerifier<NonStandardEnumUnderlyingTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task EnumWithDefaultStorage_IsClean()
    {
        const string source = """
            public enum Priority
            {
                Low,
                Normal,
                High
            }
            """;

        return CSharpAnalyzerVerifier<NonStandardEnumUnderlyingTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task EnumWithExplicitInt_IsClean()
    {
        const string source = """
            public enum Priority : int
            {
                Low,
                Normal,
                High
            }
            """;

        return CSharpAnalyzerVerifier<NonStandardEnumUnderlyingTypeAnalyzer>.VerifyAsync(source);
    }
}
