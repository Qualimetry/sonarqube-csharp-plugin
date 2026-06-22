using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ThrowInStaticConstructorAnalyzerTests
{
    [Fact]
    public Task ThrowInStaticConstructor_IsReported()
    {
        const string source = """
            using System;

            public class Config
            {
                static Config()
                {
                    {|qa_quality_throw_in_static_constructor:throw new InvalidOperationException("missing settings");|}
                }
            }
            """;

        return CSharpAnalyzerVerifier<ThrowInStaticConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StaticConstructorWithoutThrow_IsClean()
    {
        const string source = """
            public class Config
            {
                private static readonly int Retries;

                static Config()
                {
                    Retries = 3;
                }
            }
            """;

        return CSharpAnalyzerVerifier<ThrowInStaticConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ThrowInInstanceConstructor_IsClean()
    {
        const string source = """
            using System;

            public class Config
            {
                public Config(string value)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }
                }
            }
            """;

        return CSharpAnalyzerVerifier<ThrowInStaticConstructorAnalyzer>.VerifyAsync(source);
    }
}
