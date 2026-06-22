using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class DeepInheritanceAnalyzerTests
{
    [Fact]
    public Task DeepClass_IsReported()
    {
        const string source = """
            public class A { }
            public class B : A { }
            public class C : B { }
            public class {|qa_metrics_deep_inheritance:D|} : C { }
            """;

        return CSharpAnalyzerVerifier<DeepInheritanceAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_deep_inheritance.maxdepth", "2"));
    }

    [Fact]
    public Task ShallowClass_IsClean()
    {
        const string source = """
            public class Animal { }
            public class Dog : Animal { }
            """;

        return CSharpAnalyzerVerifier<DeepInheritanceAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_deep_inheritance.maxdepth", "2"));
    }
}
