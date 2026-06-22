using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NumericLiteralDigitSeparatorAnalyzer : DiagnosticAnalyzer
{
    private const int MinimumDigits = 5;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0045);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.NumericLiteralExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var literal = (LiteralExpressionSyntax)context.Node;
        var text = literal.Token.Text;
        if (!IsUngroupedDecimalInteger(text))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0045, literal.GetLocation()));
    }

    private static bool IsUngroupedDecimalInteger(string text)
    {
        var end = text.Length;
        while (end > 0 && IsSuffix(text[end - 1]))
        {
            end--;
        }

        if (end < MinimumDigits)
        {
            return false;
        }

        for (var i = 0; i < end; i++)
        {
            if (text[i] < '0' || text[i] > '9')
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsSuffix(char c) =>
        c is 'u' or 'U' or 'l' or 'L';
}
