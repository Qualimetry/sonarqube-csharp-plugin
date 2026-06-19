using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicReadonlyArrayFieldAnalyzerTests
{
    [Fact]
    public Task PublicReadonlyArray_IsReported()
    {
        const string source = """
            public class Palette
            {
                public readonly string[] {|qa_quality_public_readonly_array_field:Colors|} = { "red", "green", "blue" };
            }
            """;

        return CSharpAnalyzerVerifier<PublicReadonlyArrayFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateReadonlyArray_IsReported()
    {
        const string source = """
            public class Palette
            {
                private readonly string[] {|qa_quality_public_readonly_array_field:_colors|} = { "red", "green", "blue" };
            }
            """;

        return CSharpAnalyzerVerifier<PublicReadonlyArrayFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyListProperty_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Palette
            {
                private readonly List<string> _colors = new() { "red", "green", "blue" };

                public IReadOnlyList<string> Colors => _colors;
            }
            """;

        return CSharpAnalyzerVerifier<PublicReadonlyArrayFieldAnalyzer>.VerifyAsync(source);
    }
}
