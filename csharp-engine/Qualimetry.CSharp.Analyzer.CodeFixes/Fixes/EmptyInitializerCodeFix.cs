using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EmptyInitializerCodeFix)), Shared]
public sealed class EmptyInitializerCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("qa_style_empty_initializer");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var creation = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<BaseObjectCreationExpressionSyntax>();
        if (creation?.Initializer is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Remove empty initializer",
                _ =>
                {
                    BaseObjectCreationExpressionSyntax updated = creation.WithInitializer(null);
                    if (updated is ObjectCreationExpressionSyntax { ArgumentList: null } objectCreation)
                    {
                        updated = objectCreation
                            .WithType(objectCreation.Type.WithoutTrailingTrivia())
                            .WithArgumentList(SyntaxFactory.ArgumentList());
                    }

                    updated = updated.WithTriviaFrom(creation);
                    return Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(creation, updated)));
                },
                equivalenceKey: "qa_style_empty_initializer"),
            diagnostic);
    }
}
