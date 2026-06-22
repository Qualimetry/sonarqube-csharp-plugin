using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class HiddenBaseMethodAnalyzerTests
{
    [Fact]
    public Task NewMethodHidingBase_IsReported()
    {
        const string source = """
            public class Animal
            {
                public virtual string Describe() => "animal";
            }

            public class Dog : Animal
            {
                public new string {|qa_quality_hidden_base_method:Describe|}() => "dog";
            }
            """;

        return CSharpAnalyzerVerifier<HiddenBaseMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task OverrideMethod_IsClean()
    {
        const string source = """
            public class Animal
            {
                public virtual string Describe() => "animal";
            }

            public class Dog : Animal
            {
                public override string Describe() => "dog";
            }
            """;

        return CSharpAnalyzerVerifier<HiddenBaseMethodAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticNewMethodHidingBase_IsClean()
    {
        const string source = """
            public class Animal
            {
                public static void Configure() { }
            }

            public class Dog : Animal
            {
                public static new void Configure() { }
            }
            """;

        return CSharpAnalyzerVerifier<HiddenBaseMethodAnalyzer>.VerifyAsync(source);
    }
}
