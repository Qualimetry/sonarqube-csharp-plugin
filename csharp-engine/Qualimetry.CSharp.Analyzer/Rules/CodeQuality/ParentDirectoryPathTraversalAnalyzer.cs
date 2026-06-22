using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ParentDirectoryPathTraversalAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0026);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StringLiteralExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var literal = (LiteralExpressionSyntax)context.Node;
        var value = literal.Token.ValueText;

        if (!ContainsParentTraversal(value))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0026, literal.GetLocation()));
    }

    private static bool ContainsParentTraversal(string value)
    {
        return HasSegment(value, '/') || HasSegment(value, '\\');
    }

    private static bool HasSegment(string value, char separator)
    {
        var marker = ".." + separator;
        var index = value.IndexOf(marker, System.StringComparison.Ordinal);
        while (index >= 0)
        {
            var atStart = index == 0;
            var precededBySeparator = index > 0 && (value[index - 1] == '/' || value[index - 1] == '\\');
            if (atStart || precededBySeparator)
            {
                return true;
            }

            index = value.IndexOf(marker, index + 1, System.StringComparison.Ordinal);
        }

        return false;
    }
}
