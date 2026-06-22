using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class MutableComplexFieldAnalyzerTests
{
    [Fact]
    public Task MutableComplexField_IsReported()
    {
        const string source = """
            public class Store { }

            public class {|qa_metrics_mutable_complex_field:Cache|}
            {
                private Store _store;
            }
            """;

        return CSharpAnalyzerVerifier<MutableComplexFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReadonlyComplexField_IsClean()
    {
        const string source = """
            public class Store { }

            public class Cache
            {
                private readonly Store _store = new Store();
            }
            """;

        return CSharpAnalyzerVerifier<MutableComplexFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ConfiguredThreshold_AllowsFieldsUpToLimit()
    {
        const string source = """
            public class Store { }

            public class Cache
            {
                private Store _store;
            }
            """;

        return CSharpAnalyzerVerifier<MutableComplexFieldAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_mutable_complex_field.maxmutablecomplexfields", "1"));
    }

    [Fact]
    public Task NullablePrimitiveAndValueTypeFields_AreClean()
    {
        const string source = """
            public class Cache
            {
                private int? _count;
                private bool? _flag;
                private System.DateTime _when;
            }
            """;

        return CSharpAnalyzerVerifier<MutableComplexFieldAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ReferenceFieldStillReported_WhenValueTypesExcluded()
    {
        const string source = """
            public class Store { }

            public class {|qa_metrics_mutable_complex_field:Cache|}
            {
                private int? _count;
                private Store _store;
            }
            """;

        return CSharpAnalyzerVerifier<MutableComplexFieldAnalyzer>.VerifyAsync(source);
    }
}
