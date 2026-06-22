using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class NestedTypeMasksOuterStaticMemberAnalyzerTests
{
    [Fact]
    public Task NestedMemberMasksOuterStatic_IsReported()
    {
        const string source = """
            public class Registry
            {
                public static int Version;

                public class Entry
                {
                    public int {|qa_quality_nested_type_masks_outer_static_member:Version|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<NestedTypeMasksOuterStaticMemberAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DistinctNestedMember_IsClean()
    {
        const string source = """
            public class Registry
            {
                public static int Version;

                public class Entry
                {
                    public int EntryVersion;
                }
            }
            """;

        return CSharpAnalyzerVerifier<NestedTypeMasksOuterStaticMemberAnalyzer>.VerifyAsync(source);
    }
}
