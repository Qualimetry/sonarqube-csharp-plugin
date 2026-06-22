using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SerializableMissingConstructorAnalyzerTests
{
    [Fact]
    public Task SerializableWithoutDeserializationConstructor_IsReported()
    {
        const string source = """
            using System;
            using System.Runtime.Serialization;

            #pragma warning disable SYSLIB0051
            [Serializable]
            public class {|qa_quality_serializable_missing_constructor:Money|} : ISerializable
            {
                public void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SerializableMissingConstructorAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SerializableWithDeserializationConstructor_IsClean()
    {
        const string source = """
            using System;
            using System.Runtime.Serialization;

            #pragma warning disable SYSLIB0051
            [Serializable]
            public class Money : ISerializable
            {
                public Money()
                {
                }

                protected Money(SerializationInfo info, StreamingContext context)
                {
                }

                public void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                }
            }
            """;

        return CSharpAnalyzerVerifier<SerializableMissingConstructorAnalyzer>.VerifyAsync(source);
    }
}
