using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class EmptyInterfaceAnalyzerTests
{
    [Fact]
    public Task EmptyMarkerInterface_IsReported()
    {
        const string source = """
            public interface {|qa_quality_empty_interface:IEntity|}
            {
            }
            """;

        return CSharpAnalyzerVerifier<EmptyInterfaceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task InterfaceWithMember_IsClean()
    {
        const string source = """
            public interface IEntity
            {
                int Id { get; }
            }
            """;

        return CSharpAnalyzerVerifier<EmptyInterfaceAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CombiningInterface_IsClean()
    {
        const string source = """
            public interface IReadable
            {
                int Read();
            }

            public interface IReadableEntity : IReadable
            {
            }
            """;

        return CSharpAnalyzerVerifier<EmptyInterfaceAnalyzer>.VerifyAsync(source);
    }
}
