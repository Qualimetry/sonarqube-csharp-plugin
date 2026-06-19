using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class RedundantWhereClauseAnalyzerTests
{
    [Fact]
    public Task WhereFollowedByAny_IsReported()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class Query
            {
                public bool HasEven(List<int> numbers)
                {
                    return numbers.{|qa_quality_redundant_where_clause:Where|}(n => n % 2 == 0).Any();
                }
            }
            """;

        return CSharpAnalyzerVerifier<RedundantWhereClauseAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task PredicatePassedDirectly_IsClean()
    {
        const string source = """
            using System.Collections.Generic;
            using System.Linq;

            public class Query
            {
                public bool HasEven(List<int> numbers)
                {
                    return numbers.Any(n => n % 2 == 0);
                }
            }
            """;

        return CSharpAnalyzerVerifier<RedundantWhereClauseAnalyzer>.VerifyAsync(source);
    }
}
