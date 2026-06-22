using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MultipleTypesPerFileAnalyzerTests
{
    [Fact]
    public Task SecondTopLevelType_IsReported()
    {
        const string source = """
            public sealed class Customer
            {
            }

            public sealed class {|qa_quality_multiple_types_per_file:Order|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<MultipleTypesPerFileAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SingleTopLevelTypeWithNestedType_IsClean()
    {
        const string source = """
            public sealed class Customer
            {
                public sealed class Address
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<MultipleTypesPerFileAnalyzer>.VerifyAsync(source);
    }
}
