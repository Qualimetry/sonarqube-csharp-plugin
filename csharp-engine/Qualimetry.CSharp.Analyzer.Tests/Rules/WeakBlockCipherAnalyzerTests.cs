using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class WeakBlockCipherAnalyzerTests
{
    [Fact]
    public Task TripleDesFactory_IsReported()
    {
        string source = """
            using System.Security.Cryptography;

            public sealed class Cipher
            {
                public SymmetricAlgorithm Create() => {|qa_reliability_weak_block_cipher:TripleDES.Create|}();
            }
            """;

        return CSharpAnalyzerVerifier<WeakBlockCipherAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task AesFactory_IsClean()
    {
        string source = """
            using System.Security.Cryptography;

            public sealed class Cipher
            {
                public SymmetricAlgorithm Create() => Aes.Create();
            }
            """;

        return CSharpAnalyzerVerifier<WeakBlockCipherAnalyzer>.VerifyAsync(source);
    }
}
