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

    [Fact]
    public Task ConstantConcatenatedCommandText_IsClean()
    {
        string source = """
            public sealed class Repository
            {
                private const string Column = "Name";

                public void Find(System.Data.IDbCommand command)
                {
                    command.CommandText = "SELECT * FROM Users WHERE " + Column + " IS NOT NULL";
                }
            }
            """;

        return CSharpAnalyzerVerifier<SqlCommandInjectionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LookAlikeCommandTextTarget_IsClean()
    {
        string source = """
            public sealed class Message
            {
                public string CommandText { get; set; }

                public void Build(string name)
                {
                    CommandText = "SELECT * FROM Users WHERE Name = '" + name + "'";
                }
            }
            """;

        return CSharpAnalyzerVerifier<SqlCommandInjectionAnalyzer>.VerifyAsync(source);
    }
}
