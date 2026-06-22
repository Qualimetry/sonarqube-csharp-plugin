using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class CatchWithoutExceptionTypeAnalyzerTests
{
    [Fact]
    public Task GeneralCatch_IsReported()
    {
        const string source = """
            public class Worker
            {
                public void Run()
                {
                    try
                    {
                        Execute();
                    }
                    {|qa_quality_catch_without_exception_type:catch|}
                    {
                    }
                }

                private void Execute()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<CatchWithoutExceptionTypeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task TypedCatch_IsClean()
    {
        const string source = """
            using System;

            public class Worker
            {
                public void Run()
                {
                    try
                    {
                        Execute();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }

                private void Execute()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<CatchWithoutExceptionTypeAnalyzer>.VerifyAsync(source);
    }
}
