using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LongMethodNameAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxNameLength = 40;
    private const string Option = "qualimetry.qa_metrics_long_method_name.maxnamelength";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0020);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        var threshold = OptionReader.ReadInt(context, method.SyntaxTree, Option, DefaultMaxNameLength);

        var name = method.Identifier.ValueText;
        if (name.Length > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0020,
                method.Identifier.GetLocation(),
                name,
                name.Length.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
