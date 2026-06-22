using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

internal static class LinqQueryHelper
{
    public static bool TryGetSinglePredicateCall(
        SemanticModel semanticModel,
        ExpressionSyntax operand,
        string methodName,
        out InvocationExpressionSyntax invocation)
    {
        invocation = null!;

        var inner = operand;
        while (inner is ParenthesizedExpressionSyntax parenthesized)
        {
            inner = parenthesized.Expression;
        }

        if (inner is not InvocationExpressionSyntax candidate)
        {
            return false;
        }

        if (candidate.Expression is not MemberAccessExpressionSyntax memberAccess
            || memberAccess.Name.Identifier.ValueText != methodName)
        {
            return false;
        }

        if (candidate.ArgumentList.Arguments.Count != 1)
        {
            return false;
        }

        if (candidate.ArgumentList.Arguments[0].Expression is not LambdaExpressionSyntax)
        {
            return false;
        }

        if (semanticModel.GetSymbolInfo(candidate).Symbol is not IMethodSymbol method)
        {
            return false;
        }

        var declaringType = method.ContainingType?.ToDisplayString();
        if (declaringType != "System.Linq.Enumerable" && declaringType != "System.Linq.Queryable")
        {
            return false;
        }

        invocation = candidate;
        return true;
    }

    public static bool HasPurePredicate(InvocationExpressionSyntax invocation)
    {
        if (invocation.ArgumentList.Arguments.Count != 1
            || invocation.ArgumentList.Arguments[0].Expression is not LambdaExpressionSyntax lambda
            || lambda.Body is not { } body)
        {
            return false;
        }

        foreach (var node in body.DescendantNodesAndSelf())
        {
            switch (node)
            {
                case InvocationExpressionSyntax:
                case ObjectCreationExpressionSyntax:
                case ImplicitObjectCreationExpressionSyntax:
                case AwaitExpressionSyntax:
                case AssignmentExpressionSyntax:
                    return false;
                case PrefixUnaryExpressionSyntax prefix when IsIncrementOrDecrement(prefix.Kind()):
                    return false;
                case PostfixUnaryExpressionSyntax postfix when IsIncrementOrDecrement(postfix.Kind()):
                    return false;
            }
        }

        return true;
    }

    private static bool IsIncrementOrDecrement(SyntaxKind kind) =>
        kind is SyntaxKind.PreIncrementExpression
            or SyntaxKind.PreDecrementExpression
            or SyntaxKind.PostIncrementExpression
            or SyntaxKind.PostDecrementExpression;
}
