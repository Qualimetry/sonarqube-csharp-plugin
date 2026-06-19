using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferStringEmptyCodeFix)), Shared]
public sealed class PreferStringEmptyCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("qa_style_prefer_string_empty");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        if (root.FindNode(diagnostic.Location.SourceSpan) is not LiteralExpressionSyntax literal)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use string.Empty",
                _ =>
                {
                    var replacement = SyntaxFactory.ParseExpression("string.Empty")
                        .WithTriviaFrom(literal);
                    return Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(literal, replacement)));
                },
                equivalenceKey: "qa_style_prefer_string_empty"),
            diagnostic);
    }
}
