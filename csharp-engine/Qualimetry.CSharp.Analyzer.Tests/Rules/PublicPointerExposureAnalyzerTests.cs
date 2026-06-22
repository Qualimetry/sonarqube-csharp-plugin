using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PublicPointerExposureAnalyzerTests
{
    [Fact]
    public Task PublicNativeHandleField_IsReported()
    {
        const string source = """
            using System;

            public class DeviceContext
            {
                public IntPtr {|qa_quality_public_pointer_exposure:Handle|};
            }
            """;

        return CSharpAnalyzerVerifier<PublicPointerExposureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PrivateNativeHandleField_IsClean()
    {
        const string source = """
            using System;

            public class DeviceContext
            {
                private IntPtr _handle;

                public bool IsValid => _handle != IntPtr.Zero;
            }
            """;

        return CSharpAnalyzerVerifier<PublicPointerExposureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PublicNativeHandleFieldInInternalType_IsClean()
    {
        const string source = """
            using System;

            internal class DeviceContext
            {
                public IntPtr Handle;
            }
            """;

        return CSharpAnalyzerVerifier<PublicPointerExposureAnalyzer>.VerifyAsync(source);
    }
}
