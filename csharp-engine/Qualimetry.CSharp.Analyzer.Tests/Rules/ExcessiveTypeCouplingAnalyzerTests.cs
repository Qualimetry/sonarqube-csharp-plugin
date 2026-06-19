using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExcessiveTypeCouplingAnalyzerTests
{
    [Fact]
    public Task TypeReferencingManyTypes_IsReported()
    {
        const string source = """
            public class A { }
            public class B { }
            public class C { }

            public class {|qa_metrics_excessive_type_coupling:Widget|}
            {
                private A _a;
                private B _b;
                private C _c;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_type_coupling.maxcoupling", "2"));
    }

    [Fact]
    public Task TypeWithFewReferences_IsClean()
    {
        const string source = """
            public class A { }

            public class Widget
            {
                private A _a;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyAsync(source);
    }
}
