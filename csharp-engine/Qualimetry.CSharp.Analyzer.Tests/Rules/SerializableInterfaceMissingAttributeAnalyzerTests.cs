using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SerializableInterfaceMissingAttributeAnalyzerTests
{
    [Fact]
    public Task SerializableInterfaceWithoutAttribute_IsReported()
    {
        const string source = """
            using System.Runtime.Serialization;

            #pragma warning disable SYSLIB0051
            public class {|qa_quality_serializable_interface_missing_attribute:Ticket|} : ISerializable
            {
                public void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SerializableInterfaceMissingAttributeAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SerializableInterfaceWithAttribute_IsClean()
    {
        const string source = """
            using System;
            using System.Runtime.Serialization;

            #pragma warning disable SYSLIB0051
            [Serializable]
            public class Ticket : ISerializable
            {
                public void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SerializableInterfaceMissingAttributeAnalyzer>.VerifyAsync(source);
    }
}
