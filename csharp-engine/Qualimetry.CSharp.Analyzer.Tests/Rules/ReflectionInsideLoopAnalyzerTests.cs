using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ReflectionInsideLoopAnalyzerTests
{
    [Fact]
    public Task ReflectionCallInLoop_IsReported()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Reflection;

            public class Mapper
            {
                public void Map(List<object> items, PropertyInfo property)
                {
                    foreach (var item in items)
                    {
                        var value = {|qa_quality_reflection_inside_loop:property.GetValue(item)|};
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ReflectionInsideLoopAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DirectAccessInLoop_IsClean()
    {
        const string source = """
            using System.Collections.Generic;

            public class Mapper
            {
                public void Map(List<string> items)
                {
                    foreach (var item in items)
                    {
                        var length = item.Length;
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ReflectionInsideLoopAnalyzer>.VerifyAsync(source);
    }
}
