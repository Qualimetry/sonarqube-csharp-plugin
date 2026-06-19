using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class IndexLoopOverCollectionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0173);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ForStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var forStatement = (ForStatementSyntax)context.Node;
        if (forStatement.Declaration == null || forStatement.Declaration.Variables.Count != 1)
        {
            return;
        }

        var variable = forStatement.Declaration.Variables[0];
        if (variable.Initializer == null || !IsZeroLiteral(variable.Initializer.Value))
        {
            return;
        }

        var counter = variable.Identifier.ValueText;
        if (forStatement.Incrementors.Count != 1 || !IsIncrementOf(forStatement.Incrementors[0], counter))
        {
            return;
        }

        if (!(forStatement.Condition is BinaryExpressionSyntax condition) || !condition.IsKind(SyntaxKind.LessThanExpression))
        {
            return;
        }

        if (!(condition.Left is IdentifierNameSyntax left) || left.Identifier.ValueText != counter)
        {
            return;
        }

        var collection = GetCollectionName(condition.Right);
        if (collection == null)
        {
            return;
        }

        if (!BodyIndexes(forStatement.Statement, collection, counter))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0173, forStatement.ForKeyword.GetLocation()));
    }

    private static bool IsZeroLiteral(ExpressionSyntax expression)
    {
        return expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.NumericLiteralExpression)
            && literal.Token.ValueText == "0";
    }

    private static bool IsIncrementOf(ExpressionSyntax expression, string counter)
    {
        ExpressionSyntax? operand = null;
        if (expression is PostfixUnaryExpressionSyntax postfix && postfix.IsKind(SyntaxKind.PostIncrementExpression))
        {
            operand = postfix.Operand;
        }
        else if (expression is PrefixUnaryExpressionSyntax prefix && prefix.IsKind(SyntaxKind.PreIncrementExpression))
        {
            operand = prefix.Operand;
        }

        return operand is IdentifierNameSyntax identifier && identifier.Identifier.ValueText == counter;
    }

    private static string? GetCollectionName(ExpressionSyntax expression)
    {
        if (expression is MemberAccessExpressionSyntax memberAccess
            && memberAccess.Expression is IdentifierNameSyntax collection
            && (memberAccess.Name.Identifier.ValueText == "Length" || memberAccess.Name.Identifier.ValueText == "Count"))
        {
            return collection.Identifier.ValueText;
        }

        return null;
    }

    private static bool BodyIndexes(StatementSyntax body, string collection, string counter)
    {
        foreach (var elementAccess in body.DescendantNodes().OfType<ElementAccessExpressionSyntax>())
        {
            if (elementAccess.Expression is IdentifierNameSyntax target
                && target.Identifier.ValueText == collection
                && elementAccess.ArgumentList.Arguments.Count == 1
                && elementAccess.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax index
                && index.Identifier.ValueText == counter)
            {
                return true;
            }
        }

        return false;
    }
}
