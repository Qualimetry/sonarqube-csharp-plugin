using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ConditionalAssignmentOpportunityAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0181);

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

        var elseClause = ifStatement.Else;
        if (elseClause == null || elseClause.Statement is IfStatementSyntax)
        {
            return;
        }

        var thenAssignment = GetSingleAssignment(ifStatement.Statement);
        var elseAssignment = GetSingleAssignment(elseClause.Statement);
        if (thenAssignment == null || elseAssignment == null)
        {
            return;
        }

        if (!SameTarget(thenAssignment, elseAssignment))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0181, ifStatement.IfKeyword.GetLocation()));
    }

    private static AssignmentExpressionSyntax? GetSingleAssignment(StatementSyntax statement)
    {
        var effective = statement;
        if (statement is BlockSyntax block)
        {
            if (block.Statements.Count != 1)
            {
                return null;
            }

            effective = block.Statements[0];
        }

        if (effective is ExpressionStatementSyntax expressionStatement
            && expressionStatement.Expression is AssignmentExpressionSyntax assignment
            && assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
        {
            return assignment;
        }

        return null;
    }

    private static bool SameTarget(AssignmentExpressionSyntax first, AssignmentExpressionSyntax second)
    {
        return first.Left is IdentifierNameSyntax a
            && second.Left is IdentifierNameSyntax b
            && a.Identifier.ValueText == b.Identifier.ValueText;
    }
}
