using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ConsoleCompositeFormatAnalyzerTests
{
    [Fact]
    public Task CompositeFormatConsoleCall_IsReported()
    {
        const string source = """
            using System;

            public class Greeter
            {
                public void Hello(string name)
                {
                    {|qa_style_console_composite_format:Console.WriteLine("Hello {0}", name)|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConsoleCompositeFormatAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InterpolatedConsoleCall_IsClean()
    {
        const string source = """
            using System;

            public class Greeter
            {
                public void Hello(string name)
                {
                    Console.WriteLine($"Hello {name}");
                }
            }
            """;

        return CSharpAnalyzerVerifier<ConsoleCompositeFormatAnalyzer>.VerifyAsync(source);
    }
}
