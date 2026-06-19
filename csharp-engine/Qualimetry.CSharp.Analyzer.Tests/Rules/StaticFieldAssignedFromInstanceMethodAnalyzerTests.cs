using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class StaticFieldAssignedFromInstanceMethodAnalyzerTests
{
    [Fact]
    public Task StaticFieldWrittenFromInstanceMethod_IsReported()
    {
        const string source = """
            public sealed class Counter
            {
                private static int _total;

                public void Add(int amount)
                {
                    {|qa_quality_static_field_assigned_from_instance_method:_total|} = _total + amount;
                }
            }
            """;

        return CSharpAnalyzerVerifier<StaticFieldAssignedFromInstanceMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InstanceFieldWrittenFromInstanceMethod_IsClean()
    {
        const string source = """
            public sealed class Counter
            {
                private int _total;

                public void Add(int amount)
                {
                    _total = _total + amount;
                }
            }
            """;

        return CSharpAnalyzerVerifier<StaticFieldAssignedFromInstanceMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticFieldWrittenFromStaticMethod_IsClean()
    {
        const string source = """
            public sealed class Counter
            {
                private static int _total;

                public static void Add(int amount)
                {
                    _total = _total + amount;
                }
            }
            """;

        return CSharpAnalyzerVerifier<StaticFieldAssignedFromInstanceMethodAnalyzer>.VerifyAsync(source);
    }
}
