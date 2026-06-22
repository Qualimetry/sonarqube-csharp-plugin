using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class ExcessiveTypeCouplingAnalyzerTests
{
    [Fact]
    public Task TypeReferencingManyTypes_IsReported()
    {
        const string source = """
            public class A { }
            public class B { }
            public class C { }

            public class {|qa_metrics_excessive_type_coupling:Widget|}
            {
                private A _a;
                private B _b;
                private C _c;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_type_coupling.maxcoupling", "2"));
    }

    [Fact]
    public Task TypeWithFewReferences_IsClean()
    {
        const string source = """
            public class A { }

            public class Widget
            {
                private A _a;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task GenericParametersAndPrimitives_DoNotInflateCoupling()
    {
        const string source = """
            using System.Collections.Generic;

            public class A { }

            public class Widget<TKey, TValue>
            {
                private int _count;
                private string _name;
                private TKey _key;
                private TValue _value;
                private Dictionary<TKey, TValue> _map;
                private A _a;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_type_coupling.maxcoupling", "2"));
    }

    [Fact]
    public Task DistinctNamedTypesInGenerics_AreCounted()
    {
        const string source = """
            using System.Collections.Generic;

            public class A { }
            public class B { }
            public class C { }

            public class {|qa_metrics_excessive_type_coupling:Widget|}
            {
                private List<A> _a;
                private B _b;
                private C _c;
            }
            """;

        return CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyAsync(
            source, ("qualimetry.qa_metrics_excessive_type_coupling.maxcoupling", "2"));
    }
}
