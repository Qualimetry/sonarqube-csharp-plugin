using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class InsecureRandomForSecurityAnalyzerTests
{
    [Fact]
    public Task RandomBoundToSecurityName_IsReported()
    {
        string source = """
            using System;

            public sealed class Tokens
            {
                public int Next()
                {
                    var tokenSeed = {|qa_reliability_insecure_random_for_security:new Random()|};
                    return tokenSeed.Next();
                }
            }
            """;

        return CSharpAnalyzerVerifier<InsecureRandomForSecurityAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task RandomForOrdinaryUse_IsClean()
    {
        string source = """
            using System;

            public sealed class Shuffle
            {
                public int Pick()
                {
                    var rng = new Random();
                    return rng.Next();
                }
            }
            """;

        return CSharpAnalyzerVerifier<InsecureRandomForSecurityAnalyzer>.VerifyAsync(source);
    }
}
