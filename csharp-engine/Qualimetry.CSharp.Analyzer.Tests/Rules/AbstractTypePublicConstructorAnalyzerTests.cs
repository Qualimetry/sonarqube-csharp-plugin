using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Design;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class AbstractTypePublicConstructorAnalyzerTests
{
    [Fact]
    public Task PublicConstructorOnAbstractType_IsReported()
    {
        const string source = """
            public abstract class Shape
            {
                {|qa_quality_abstract_type_public_constructor:public|} Shape(string name)
                {
                    Name = name;
                }

                public string Name { get; }
            }
            """;

        return CSharpAnalyzerVerifier<AbstractTypePublicConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ProtectedConstructorOnAbstractType_IsClean()
    {
        const string source = """
            public abstract class Shape
            {
                protected Shape(string name)
                {
                    Name = name;
                }

                public string Name { get; }
            }
            """;

        return CSharpAnalyzerVerifier<AbstractTypePublicConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicConstructorOnConcreteType_IsClean()
    {
        const string source = """
            public class Shape
            {
                public Shape()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<AbstractTypePublicConstructorAnalyzer>.VerifyAsync(source);
    }
}
