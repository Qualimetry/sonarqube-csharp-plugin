using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NullGuardToConditionalAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0182);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IfStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;
        if (ifStatement.Else != null)
        {
            return;
        }

        var guarded = GetGuardedIdentifier(ifStatement.Condition);
        if (guarded == null)
        {
            return;
        }

        var statement = GetSingleStatement(ifStatement.Statement);
        if (!(statement is ExpressionStatementSyntax expressionStatement))
        {
            return;
        }

        if (!InvokesOn(expressionStatement.Expression, guarded))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0182, ifStatement.IfKeyword.GetLocation()));
    }

    private static string? GetGuardedIdentifier(ExpressionSyntax condition)
    {
        if (!(condition is BinaryExpressionSyntax binary) || !binary.IsKind(SyntaxKind.NotEqualsExpression))
        {
            return null;
        }

        if (binary.Left is IdentifierNameSyntax left && binary.Right.IsKind(SyntaxKind.NullLiteralExpression))
        {
            return left.Identifier.ValueText;
        }

        if (binary.Right is IdentifierNameSyntax right && binary.Left.IsKind(SyntaxKind.NullLiteralExpression))
        {
            return right.Identifier.ValueText;
        }

        return null;
    }

    private static StatementSyntax? GetSingleStatement(StatementSyntax statement)
    {
        if (statement is BlockSyntax block)
        {
            return block.Statements.Count == 1 ? block.Statements[0] : null;
        }

        return statement;
    }

    private static bool InvokesOn(ExpressionSyntax expression, string identifier)
    {
        if (!(expression is InvocationExpressionSyntax invocation))
        {
            return false;
        }

        switch (invocation.Expression)
        {
            case IdentifierNameSyntax id:
                return id.Identifier.ValueText == identifier;
            case MemberAccessExpressionSyntax memberAccess:
                return memberAccess.Expression is IdentifierNameSyntax root && root.Identifier.ValueText == identifier;
            default:
                return false;
        }
    }
}
