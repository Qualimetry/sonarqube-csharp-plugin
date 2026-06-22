using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BooleanReturnViaIfAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0147);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IfStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;
        if (ifStatement.Parent is ElseClauseSyntax)
        {
            return;
        }

        if (!TryGetReturnedBool(ifStatement.Statement, out var thenValue))
        {
            return;
        }

        bool elseValue;
        if (ifStatement.Else != null)
        {
            if (!TryGetReturnedBool(ifStatement.Else.Statement, out elseValue))
            {
                return;
            }
        }
        else if (!TryGetFollowingReturnedBool(ifStatement, out elseValue))
        {
            return;
        }

        if (thenValue == elseValue)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0147, ifStatement.IfKeyword.GetLocation()));
    }

    private static bool TryGetReturnedBool(StatementSyntax statement, out bool value)
    {
        value = false;
        var effective = statement;
        if (statement is BlockSyntax block)
        {
            if (block.Statements.Count != 1)
            {
                return false;
            }

            effective = block.Statements[0];
        }

        if (effective is ReturnStatementSyntax returnStatement && returnStatement.Expression != null)
        {
            return TryGetBoolLiteral(returnStatement.Expression, out value);
        }

        return false;
    }

    private static bool TryGetFollowingReturnedBool(IfStatementSyntax ifStatement, out bool value)
    {
        value = false;
        if (ifStatement.Parent is BlockSyntax parent)
        {
            var index = parent.Statements.IndexOf(ifStatement);
            if (index >= 0 && index + 1 < parent.Statements.Count
                && parent.Statements[index + 1] is ReturnStatementSyntax returnStatement
                && returnStatement.Expression != null)
            {
                return TryGetBoolLiteral(returnStatement.Expression, out value);
            }
        }

        return false;
    }

    private static bool TryGetBoolLiteral(ExpressionSyntax expression, out bool value)
    {
        switch (expression.Kind())
        {
            case SyntaxKind.TrueLiteralExpression:
                value = true;
                return true;
            case SyntaxKind.FalseLiteralExpression:
                value = false;
                return true;
            default:
                value = false;
                return false;
        }
    }
}
