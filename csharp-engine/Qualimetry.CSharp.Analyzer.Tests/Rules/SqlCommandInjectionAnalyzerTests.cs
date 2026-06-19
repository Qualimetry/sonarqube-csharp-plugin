using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Reliability;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class SqlCommandInjectionAnalyzerTests
{
    [Fact]
    public Task ConcatenatedCommandText_IsReported()
    {
        string source = """
            public sealed class Repository
            {
                public void Find(System.Data.IDbCommand command, string name)
                {
                    command.CommandText = {|qa_reliability_sql_command_injection:"SELECT * FROM Users WHERE Name = '" + name + "'"|};
                }
            }
            """;

        return CSharpAnalyzerVerifier<SqlCommandInjectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ParameterizedCommandText_IsClean()
    {
        string source = """
            public sealed class Repository
            {
                public void Find(System.Data.IDbCommand command, string name)
                {
                    command.CommandText = "SELECT * FROM Users WHERE Name = @name";
                }
            }
            """;

        return CSharpAnalyzerVerifier<SqlCommandInjectionAnalyzer>.VerifyAsync(source);
    }
}
