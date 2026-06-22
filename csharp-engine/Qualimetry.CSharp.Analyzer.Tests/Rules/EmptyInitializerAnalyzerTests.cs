using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyInitializerAnalyzerTests
{
    [Fact]
    public Task EmptyCollectionInitializer_IsReported()
    {
        const string source = """
            using System.Collections.Generic;

            public class C
            {
                public void M()
                {
                    var list = new List<int>() {|qa_style_empty_initializer:{ }|};
                    list.Add(1);
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyInitializerAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NoInitializer_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class C
            {
                public void M()
                {
                    var list = new List<int>();
                    list.Add(1);
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyInitializerAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PopulatedInitializer_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class C
            {
                public void M()
                {
                    var list = new List<int> { 1, 2 };
                    list.Add(3);
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyInitializerAnalyzer>.VerifyAsync(source);
    }
}
