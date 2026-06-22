using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantDefaultFieldInitializerCodeFix)), Shared]
public sealed class RedundantDefaultFieldInitializerCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("qa_style_redundant_default_field_initializer");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        var declarator = node.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
        if (declarator?.Initializer is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Remove redundant initializer",
                _ =>
                {
                    var updated = declarator
                        .WithInitializer(null)
                        .WithIdentifier(declarator.Identifier.WithTrailingTrivia());
                    return Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(declarator, updated)));
                },
                equivalenceKey: "qa_style_redundant_default_field_initializer"),
            diagnostic);
    }
}
