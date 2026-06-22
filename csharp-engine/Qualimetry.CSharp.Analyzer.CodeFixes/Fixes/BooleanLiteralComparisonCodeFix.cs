using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BooleanLiteralComparisonCodeFix)), Shared]
public sealed class BooleanLiteralComparisonCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("qa_style_boolean_literal_comparison");

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];
        var binary = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<BinaryExpressionSyntax>();
        if (binary is null)
        {
            return;
        }

        var leftIsLiteral = IsBooleanLiteral(binary.Left, out var leftValue);
        var rightIsLiteral = IsBooleanLiteral(binary.Right, out var rightValue);
        if (leftIsLiteral == rightIsLiteral)
        {
            return;
        }

        var operand = leftIsLiteral ? binary.Right : binary.Left;
        var literalValue = leftIsLiteral ? leftValue : rightValue;
        var isEquals = binary.IsKind(SyntaxKind.EqualsExpression);
        var keepAsIs = isEquals == literalValue;

        context.RegisterCodeFix(
            CodeAction.Create(
                "Remove comparison to boolean literal",
                _ =>
                {
                    ExpressionSyntax replacement = keepAsIs
                        ? operand.WithoutTrivia()
                        : SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, Parenthesize(operand.WithoutTrivia()));
                    replacement = replacement.WithTriviaFrom(binary);
                    return Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(binary, replacement)));
                },
                equivalenceKey: "qa_style_boolean_literal_comparison"),
            diagnostic);
    }

    private static ExpressionSyntax Parenthesize(ExpressionSyntax expression) =>
        expression is IdentifierNameSyntax or MemberAccessExpressionSyntax or InvocationExpressionSyntax or ParenthesizedExpressionSyntax or ElementAccessExpressionSyntax
            ? expression
            : SyntaxFactory.ParenthesizedExpression(expression);

    private static bool IsBooleanLiteral(ExpressionSyntax expression, out bool value)
    {
        if (expression.IsKind(SyntaxKind.TrueLiteralExpression))
        {
            value = true;
            return true;
        }

        if (expression.IsKind(SyntaxKind.FalseLiteralExpression))
        {
            value = false;
            return true;
        }

        value = false;
        return false;
    }
}
