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

    [Fact]
    public Task FlagsEnumWithByteStorage_IsClean()
    {
        const string source = """
            using System;

            [Flags]
            public enum Permission : byte
            {
                None = 0,
                Read = 1,
                Write = 2,
                Execute = 4
            }
            """;

        return CSharpAnalyzerVerifier<NonStandardEnumUnderlyingTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonFlagsEnumWithByteStorage_IsReported()
    {
        const string source = """
            public enum Priority : {|qa_quality_non_standard_enum_underlying_type:byte|}
            {
                Low,
                Normal,
                High
            }
            """;

        return CSharpAnalyzerVerifier<NonStandardEnumUnderlyingTypeAnalyzer>.VerifyAsync(source);
    }
}
