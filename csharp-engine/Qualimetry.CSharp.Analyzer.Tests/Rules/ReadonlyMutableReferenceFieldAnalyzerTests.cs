using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ReadonlyMutableReferenceFieldAnalyzerTests
{
    [Fact]
    public Task ReadonlyListField_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public sealed class Registry
            {
                private readonly List<string> {|qa_quality_readonly_mutable_reference_field:_names|} = new List<string>();
            }
            """;

        return CSharpAnalyzerVerifier<ReadonlyMutableReferenceFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyReadOnlyListField_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public sealed class Registry
            {
                private readonly IReadOnlyList<string> _names = new List<string>();
            }
            """;

        return CSharpAnalyzerVerifier<ReadonlyMutableReferenceFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyValueTypeField_IsClean()
    {
        const string source = """
            public sealed class Registry
            {
                private readonly int _count = 0;
            }
            """;

        return CSharpAnalyzerVerifier<ReadonlyMutableReferenceFieldAnalyzer>.VerifyAsync(source);
    }
}
