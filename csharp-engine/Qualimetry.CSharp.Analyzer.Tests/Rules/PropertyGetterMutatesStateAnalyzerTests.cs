using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PropertyGetterMutatesStateAnalyzerTests
{
    [Fact]
    public Task GetterThatMutatesField_IsReported()
    {
        const string source = """
            public class Sequence
            {
                private int _value;

                public int Next
                {
                    {|qa_quality_property_getter_mutates_state:get|}
                    {
                        _value += 1;
                        return _value;
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<PropertyGetterMutatesStateAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PureGetter_IsClean()
    {
        const string source = """
            public class Sequence
            {
                private int _value;

                public int Current
                {
                    get
                    {
                        return _value;
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<PropertyGetterMutatesStateAnalyzer>.VerifyAsync(source);
    }
}
