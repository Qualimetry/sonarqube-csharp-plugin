using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyCatchBlockAnalyzerTests
{
    [Fact]
    public Task EmptyCatch_IsReported()
    {
        const string source = """
            using System;

            public class C
            {
                public void M()
                {
                    try
                    {
                        DoWork();
                    }
                    {|qa_quality_empty_catch_block:catch|} (InvalidOperationException)
                    {
                    }
                }

                private static void DoWork()
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyCatchBlockAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task HandledCatch_IsClean()
    {
        const string source = """
            using System;

            public class C
            {
                public void M()
                {
                    try
                    {
                        DoWork();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Record(ex);
                    }
                }

                private static void DoWork()
                {
                }

                private static void Record(Exception ex)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyCatchBlockAnalyzer>.VerifyAsync(source);
    }
}
