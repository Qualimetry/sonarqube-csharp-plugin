using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Qualimetry.CSharp.Analyzer.Rules.Naming;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

// Every configurable rule must honour the parameter SonarScanner for .NET delivers in SonarLint.xml.
// Each test asserts the analyzer is clean at the built-in default and that overriding the parameter
// through SonarLint.xml changes the outcome. A parameter the analyzer ignores fails one of these tests.
public class ConfigurableParameterTests
{
    private static string Xml(string ruleId, string key, string value) =>
        "<AnalysisInput><Rules><Rule><Key>" + ruleId + "</Key><Parameters><Parameter><Key>"
        + key + "</Key><Value>" + value + "</Value></Parameter></Parameters></Rule></Rules></AnalysisInput>";

    [Fact]
    public async Task QCS0003_Suffix_HonorsParam()
    {
        await CSharpAnalyzerVerifier<AbstractBaseClassSuffixAnalyzer>.VerifyAsync(
            "public abstract class RepositoryBase\n{\n}\n");
        await CSharpAnalyzerVerifier<AbstractBaseClassSuffixAnalyzer>.VerifyWithSonarLintAsync(
            "public abstract class {|qa_naming_abstract_base_class_suffix:RepositoryBase|}\n{\n}\n",
            Xml("qa_naming_abstract_base_class_suffix", "suffix", "Component"));
    }

    [Fact]
    public async Task QCS0013_TypeCoupling_HonorsParam()
    {
        const string body = "public class A1 {{}}\npublic class A2 {{}}\npublic class A3 {{}}\npublic class {0}\n{{\n    public A1 a;\n    public A2 b;\n    public A3 c;\n}}\n";
        await CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyAsync(
            string.Format(body, "C"));
        await CSharpAnalyzerVerifier<ExcessiveTypeCouplingAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_excessive_type_coupling:C|}"),
            Xml("qa_metrics_excessive_type_coupling", "maxcoupling", "2"));
    }

    [Fact]
    public async Task QCS0014_FieldName_HonorsParam()
    {
        const string body = "public class C\n{{\n    public int {0};\n}}\n";
        await CSharpAnalyzerVerifier<LongFieldNameAnalyzer>.VerifyAsync(
            string.Format(body, "abcdefghij"));
        await CSharpAnalyzerVerifier<LongFieldNameAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_long_field_name:abcdefghij|}"),
            Xml("qa_metrics_long_field_name", "maxnamelength", "5"));
    }

    [Fact]
    public async Task QCS0017_LargeInterface_HonorsParam()
    {
        const string body = "public interface {0}\n{{\n    void A();\n    void B();\n}}\n";
        await CSharpAnalyzerVerifier<LargeInterfaceAnalyzer>.VerifyAsync(
            string.Format(body, "I"));
        await CSharpAnalyzerVerifier<LargeInterfaceAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_large_interface:I|}"),
            Xml("qa_metrics_large_interface", "maxmembers", "1"));
    }

    [Fact]
    public async Task QCS0019_LongMethod_HonorsParam()
    {
        const string body = "public class C\n{{\n    public void {0}()\n    {{\n        int a = 1;\n        int b = 2;\n    }}\n}}\n";
        await CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyAsync(
            string.Format(body, "M"));
        await CSharpAnalyzerVerifier<LongMethodAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_long_method:M|}"),
            Xml("qa_metrics_long_method", "maxstatements", "1"));
    }

    [Fact]
    public async Task QCS0020_MethodName_HonorsParam()
    {
        const string body = "public class C\n{{\n    public void {0}() {{ }}\n}}\n";
        await CSharpAnalyzerVerifier<LongMethodNameAnalyzer>.VerifyAsync(
            string.Format(body, "Abcdefghij"));
        await CSharpAnalyzerVerifier<LongMethodNameAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_long_method_name:Abcdefghij|}"),
            Xml("qa_metrics_long_method_name", "maxnamelength", "5"));
    }

    [Fact]
    public async Task QCS0021_MethodOverloads_HonorsParam()
    {
        const string body = "public class {0}\n{{\n    public void M() {{ }}\n    public void M(int x) {{ }}\n}}\n";
        await CSharpAnalyzerVerifier<ExcessiveMethodOverloadsAnalyzer>.VerifyAsync(
            string.Format(body, "C"));
        await CSharpAnalyzerVerifier<ExcessiveMethodOverloadsAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_excessive_method_overloads:C|}"),
            Xml("qa_metrics_excessive_method_overloads", "maxoverloads", "1"));
    }

    [Fact]
    public async Task QCS0022_ParameterCount_HonorsParam()
    {
        const string body = "public class C\n{{\n    public void {0}(int a, int b) {{ }}\n}}\n";
        await CSharpAnalyzerVerifier<ExcessiveParameterCountAnalyzer>.VerifyAsync(
            string.Format(body, "M"));
        await CSharpAnalyzerVerifier<ExcessiveParameterCountAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_excessive_parameter_count:M|}"),
            Xml("qa_metrics_excessive_parameter_count", "maxparameters", "1"));
    }

    [Fact]
    public async Task QCS0029_LargeType_HonorsParam()
    {
        const string body = "public class {0}\n{{\n}}\n";
        await CSharpAnalyzerVerifier<LargeTypeAnalyzer>.VerifyAsync(
            string.Format(body, "C"));
        await CSharpAnalyzerVerifier<LargeTypeAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_large_type:C|}"),
            Xml("qa_metrics_large_type", "maxlines", "1"));
    }

    [Fact]
    public async Task QCS0030_TypeName_HonorsParam()
    {
        await CSharpAnalyzerVerifier<LongTypeNameAnalyzer>.VerifyAsync(
            "public class Abcdefghij\n{\n}\n");
        await CSharpAnalyzerVerifier<LongTypeNameAnalyzer>.VerifyWithSonarLintAsync(
            "public class {|qa_metrics_long_type_name:Abcdefghij|}\n{\n}\n",
            Xml("qa_metrics_long_type_name", "maxnamelength", "5"));
    }

    [Fact]
    public async Task QCS0031_FieldCount_HonorsParam()
    {
        const string body = "public class {0}\n{{\n    public int a;\n    public int b;\n}}\n";
        await CSharpAnalyzerVerifier<ExcessiveFieldCountAnalyzer>.VerifyAsync(
            string.Format(body, "C"));
        await CSharpAnalyzerVerifier<ExcessiveFieldCountAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_excessive_field_count:C|}"),
            Xml("qa_metrics_excessive_field_count", "maxfields", "1"));
    }

    [Fact]
    public async Task QCS0032_MethodCount_HonorsParam()
    {
        const string body = "public class {0}\n{{\n    public void A() {{ }}\n    public void B() {{ }}\n}}\n";
        await CSharpAnalyzerVerifier<ExcessiveMethodCountAnalyzer>.VerifyAsync(
            string.Format(body, "C"));
        await CSharpAnalyzerVerifier<ExcessiveMethodCountAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_excessive_method_count:C|}"),
            Xml("qa_metrics_excessive_method_count", "maxmethods", "1"));
    }

    [Fact]
    public async Task QCS0033_CyclomaticComplexity_HonorsParam()
    {
        const string body = "public class C\n{{\n    public void {0}(bool b)\n    {{\n        if (b) {{ }}\n    }}\n}}\n";
        await CSharpAnalyzerVerifier<HighCyclomaticComplexityAnalyzer>.VerifyAsync(
            string.Format(body, "M"));
        await CSharpAnalyzerVerifier<HighCyclomaticComplexityAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_high_cyclomatic_complexity:M|}"),
            Xml("qa_metrics_high_cyclomatic_complexity", "maxcomplexity", "1"));
    }

    [Fact]
    public async Task QCS0047_DeepInheritance_HonorsParam()
    {
        const string body = "public class A {{ }}\npublic class B : A {{ }}\npublic class {0} : B {{ }}\n";
        await CSharpAnalyzerVerifier<DeepInheritanceAnalyzer>.VerifyAsync(
            string.Format(body, "C"));
        await CSharpAnalyzerVerifier<DeepInheritanceAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_deep_inheritance:C|}"),
            Xml("qa_metrics_deep_inheritance", "maxdepth", "1"));
    }

    [Fact]
    public async Task QCS0052_MutableComplexField_HonorsParam()
    {
        // Default threshold is 0, so a single mutable complex field already reports (on the type).
        // Raising the parameter to 1 must suppress it - the inverse behaviour change.
        const string body = "public class A1 {{ }}\npublic class {0}\n{{\n    public A1 a;\n}}\n";
        await CSharpAnalyzerVerifier<MutableComplexFieldAnalyzer>.VerifyAsync(
            string.Format(body, "{|qa_metrics_mutable_complex_field:C|}"));
        await CSharpAnalyzerVerifier<MutableComplexFieldAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "C"),
            Xml("qa_metrics_mutable_complex_field", "maxmutablecomplexfields", "1"));
    }

    [Fact]
    public async Task QCS0068_FieldAssignedFromManyMethods_HonorsParam()
    {
        const string body = "public class C\n{{\n    private int {0};\n    public void A() {{ f = 1; }}\n    public void B() {{ f = 2; }}\n    public int Read() => f;\n}}\n";
        await CSharpAnalyzerVerifier<FieldAssignedFromManyMethodsAnalyzer>.VerifyAsync(
            string.Format(body, "f"));
        await CSharpAnalyzerVerifier<FieldAssignedFromManyMethodsAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_field_assigned_from_many_methods:f|}"),
            Xml("qa_metrics_field_assigned_from_many_methods", "maxassigningmethods", "1"));
    }

    [Fact]
    public async Task QCS0099_InstanceState_HonorsParam()
    {
        const string body = "public class {0}\n{{\n    public int a;\n    public int b;\n}}\n";
        await CSharpAnalyzerVerifier<ExcessiveInstanceStateAnalyzer>.VerifyAsync(
            string.Format(body, "C"));
        await CSharpAnalyzerVerifier<ExcessiveInstanceStateAnalyzer>.VerifyWithSonarLintAsync(
            string.Format(body, "{|qa_metrics_excessive_instance_state:C|}"),
            Xml("qa_metrics_excessive_instance_state", "maxinstancefields", "1"));
    }
}
