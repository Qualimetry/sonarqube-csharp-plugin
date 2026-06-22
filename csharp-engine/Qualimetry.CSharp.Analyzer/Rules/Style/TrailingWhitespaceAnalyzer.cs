using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TrailingWhitespaceAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0146);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxTreeAction(Analyze);
    }

    private static void Analyze(SyntaxTreeAnalysisContext context)
    {
        var text = context.Tree.GetText(context.CancellationToken);
        foreach (var line in text.Lines)
        {
            var span = line.Span;
            if (span.IsEmpty)
            {
                continue;
            }

            var content = text.ToString(span);
            var trimmedLength = content.Length;
            while (trimmedLength > 0 && (content[trimmedLength - 1] == ' ' || content[trimmedLength - 1] == '\t'))
            {
                trimmedLength--;
            }

            if (trimmedLength == content.Length)
            {
                continue;
            }

            var location = Location.Create(context.Tree, TextSpan.FromBounds(span.Start + trimmedLength, span.End));
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0146, location));
        }
    }
}
