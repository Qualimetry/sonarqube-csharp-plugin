using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class VirtualCallInConstructorAnalyzerTests
{
    [Fact]
    public Task VirtualCallFromConstructor_IsReported()
    {
        const string source = """
            public abstract class Document
            {
                protected Document()
                {
                    {|qa_quality_virtual_call_in_constructor:Render()|};
                }

                protected abstract void Render();
            }
            """;

        return CSharpAnalyzerVerifier<VirtualCallInConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NonVirtualCallFromConstructor_IsClean()
    {
        const string source = """
            public abstract class Document
            {
                protected Document()
                {
                    Prepare();
                }

                public void Initialize() => Render();

                private void Prepare()
                {
                }

                protected abstract void Render();
            }
            """;

        return CSharpAnalyzerVerifier<VirtualCallInConstructorAnalyzer>.VerifyAsync(source);
    }
}
