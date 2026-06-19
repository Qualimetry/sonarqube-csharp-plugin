using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.CodeQuality;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class UriMemberAsStringAnalyzerTests
{
    [Fact]
    public Task StringPropertyNamedUrl_IsReported()
    {
        const string source = """
            public class Endpoint
            {
                public string {|qa_quality_uri_member_as_string:ServiceUrl|} { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<UriMemberAsStringAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task UriTypedProperty_IsClean()
    {
        const string source = """
            using System;

            public class Endpoint
            {
                public Uri ServiceUrl { get; set; }
            }
            """;

        return CSharpAnalyzerVerifier<UriMemberAsStringAnalyzer>.VerifyAsync(source);
    }
}
