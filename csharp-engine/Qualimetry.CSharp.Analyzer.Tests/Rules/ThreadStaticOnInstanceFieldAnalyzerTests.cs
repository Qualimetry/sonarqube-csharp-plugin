using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ThreadStaticOnInstanceFieldAnalyzerTests
{
    [Fact]
    public Task ThreadStaticOnInstanceField_IsReported()
    {
        const string source = """
            using System;

            public class Context
            {
                [{|qa_quality_thread_static_on_instance_field:ThreadStatic|}]
                private int depth;
            }
            """;

        return CSharpAnalyzerVerifier<ThreadStaticOnInstanceFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ThreadStaticOnStaticField_IsClean()
    {
        const string source = """
            using System;

            public class Context
            {
                [ThreadStatic]
                private static int depth;
            }
            """;

        return CSharpAnalyzerVerifier<ThreadStaticOnInstanceFieldAnalyzer>.VerifyAsync(source);
    }
}
