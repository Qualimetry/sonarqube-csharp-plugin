using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EmptyMethodSummaryAnalyzer : DiagnosticAnalyzer
{
    private const string OpenTag = "<summary>";
    private const string CloseTag = "</summary>";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0018);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        var documentation = GetDocumentationText(method);
        if (documentation is null)
        {
            return;
        }

        if (!HasEmptySummary(documentation))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0018, method.Identifier.GetLocation()));
    }

    private static string? GetDocumentationText(MethodDeclarationSyntax method)
    {
        var pieces = method.GetLeadingTrivia()
            .Where(IsDocumentationTrivia)
            .Select(t => t.ToString())
            .ToList();

        return pieces.Count == 0 ? null : string.Join("\n", pieces);
    }

    private static bool IsDocumentationTrivia(SyntaxTrivia trivia)
    {
        if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
            || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
        {
            return true;
        }

        return trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) && trivia.ToString().StartsWith("///", StringComparison.Ordinal);
    }

    private static bool HasEmptySummary(string documentation)
    {
        var open = documentation.IndexOf(OpenTag, StringComparison.Ordinal);
        if (open < 0)
        {
            return false;
        }

        var contentStart = open + OpenTag.Length;
        var close = documentation.IndexOf(CloseTag, contentStart, StringComparison.Ordinal);
        if (close < 0)
        {
            return false;
        }

        var inner = documentation.Substring(contentStart, close - contentStart)
            .Replace("/", string.Empty)
            .Replace("*", string.Empty);

        return string.IsNullOrWhiteSpace(inner);
    }
}
