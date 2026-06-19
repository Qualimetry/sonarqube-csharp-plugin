using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExplicitBackingFieldPropertyAnalyzerTests
{
    [Fact]
    public Task PropertyWrappingBackingField_IsReported()
    {
        const string source = """
            public class Person
            {
                private string _name;

                public string {|qa_style_explicit_backing_field_property:Name|}
                {
                    get { return _name; }
                    set { _name = value; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExplicitBackingFieldPropertyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AutoProperty_IsClean()
    {
        const string source = """
            public class Person
            {
                public string Name { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<ExplicitBackingFieldPropertyAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PropertyWithLogic_IsClean()
    {
        const string source = """
            public class Person
            {
                private string _name;

                public string Name
                {
                    get { return _name; }
                    set { _name = value ?? string.Empty; }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ExplicitBackingFieldPropertyAnalyzer>.VerifyAsync(source);
    }
}
