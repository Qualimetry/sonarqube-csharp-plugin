using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class VirtualPropertyAccessInConstructorAnalyzerTests
{
    [Fact]
    public Task OverridablePropertyReadInConstructor_IsReported()
    {
        const string source = """
            public abstract class View
            {
                private readonly string _label;

                protected View()
                {
                    _label = {|qa_quality_virtual_property_access_in_constructor:Caption|};
                }

                public string Label => _label;

                protected abstract string Caption { get; }
            }
            """;

        return CSharpAnalyzerVerifier<VirtualPropertyAccessInConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConstructorArgument_IsClean()
    {
        const string source = """
            public abstract class View
            {
                private readonly string _label;

                protected View(string caption)
                {
                    _label = caption;
                }

                public string Label => _label;

                protected abstract string Caption { get; }
            }
            """;

        return CSharpAnalyzerVerifier<VirtualPropertyAccessInConstructorAnalyzer>.VerifyAsync(source);
    }
}
