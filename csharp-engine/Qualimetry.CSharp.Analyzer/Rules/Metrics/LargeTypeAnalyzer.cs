using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LargeTypeAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxLines = 400;
    private const string Option = "qualimetry.qa_metrics_large_type.maxlines";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0029);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.InterfaceDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;
        var threshold = OptionReader.ReadInt(context, declaration.SyntaxTree, Option, DefaultMaxLines);

        var span = declaration.GetLocation().GetLineSpan();
        var lines = span.EndLinePosition.Line - span.StartLinePosition.Line + 1;

        if (lines > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0029,
                declaration.Identifier.GetLocation(),
                lines.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
