using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferVerbatimStringAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0183);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StringLiteralExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var literal = (LiteralExpressionSyntax)context.Node;
        var text = literal.Token.Text;
        if (text.Length == 0 || text[0] == '@')
        {
            return;
        }

        if (!OnlyBackslashEscapes(text))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0183, literal.GetLocation()));
    }

    private static bool OnlyBackslashEscapes(string tokenText)
    {
        var hasBackslashEscape = false;
        for (var i = 0; i < tokenText.Length; i++)
        {
            if (tokenText[i] != '\\')
            {
                continue;
            }

            if (i + 1 >= tokenText.Length || tokenText[i + 1] != '\\')
            {
                return false;
            }

            hasBackslashEscape = true;
            i++;
        }

        return hasBackslashEscape;
    }
}
