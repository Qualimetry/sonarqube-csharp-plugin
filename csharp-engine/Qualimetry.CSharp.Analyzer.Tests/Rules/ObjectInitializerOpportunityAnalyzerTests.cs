using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ObjectInitializerOpportunityAnalyzerTests
{
    [Fact]
    public Task CreationFollowedByPropertyAssignment_IsReported()
    {
        const string source = """
            public class Widget
            {
                public int Width { get; set; }
            }

            public class C
            {
                public Widget Build()
                {
                    var widget = {|qa_style_object_initializer_opportunity:new Widget()|};
                    widget.Width = 10;
                    return widget;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ObjectInitializerOpportunityAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ObjectInitializer_IsClean()
    {
        const string source = """
            public class Widget
            {
                public int Width { get; set; }
            }

            public class C
            {
                public Widget Build()
                {
                    var widget = new Widget
                    {
                        Width = 10,
                    };
                    return widget;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ObjectInitializerOpportunityAnalyzer>.VerifyAsync(source);
    }
}
