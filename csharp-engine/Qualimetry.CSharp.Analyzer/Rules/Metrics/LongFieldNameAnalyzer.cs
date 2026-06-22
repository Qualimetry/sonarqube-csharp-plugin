using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LongFieldNameAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxNameLength = 40;
    private const string Option = "qualimetry.qa_metrics_long_field_name.maxnamelength";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0014);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;
        var threshold = OptionReader.ReadInt(context, field.SyntaxTree, Option, DefaultMaxNameLength);

        foreach (var variable in field.Declaration.Variables)
        {
            var name = variable.Identifier.ValueText;
            if (name.Length > threshold)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0014,
                    variable.Identifier.GetLocation(),
                    name,
                    name.Length.ToString(CultureInfo.InvariantCulture),
                    threshold.ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}
