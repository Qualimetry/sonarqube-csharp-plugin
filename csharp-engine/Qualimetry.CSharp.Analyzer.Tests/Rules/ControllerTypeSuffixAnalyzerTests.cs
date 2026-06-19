using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ControllerTypeSuffixAnalyzerTests
{
    [Fact]
    public Task ControllerWithoutSuffix_IsReported()
    {
        const string source = """
            public class ControllerBase
            {
            }

            public class {|qa_naming_controller_type_suffix:Account|} : ControllerBase
            {
            }
            """;

        return CSharpAnalyzerVerifier<ControllerTypeSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ControllerWithSuffix_IsClean()
    {
        const string source = """
            public class ControllerBase
            {
            }

            public class AccountController : ControllerBase
            {
            }
            """;

        return CSharpAnalyzerVerifier<ControllerTypeSuffixAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UnrelatedClass_IsClean()
    {
        const string source = """
            public class Account
            {
            }
            """;

        return CSharpAnalyzerVerifier<ControllerTypeSuffixAnalyzer>.VerifyAsync(source);
    }
}
