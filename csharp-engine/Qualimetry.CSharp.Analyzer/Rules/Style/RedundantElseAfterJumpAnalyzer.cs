using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RedundantElseAfterJumpAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0145);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IfStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;
        var elseClause = ifStatement.Else;
        if (elseClause == null)
        {
            return;
        }

        if (elseClause.Statement is IfStatementSyntax)
        {
            return;
        }

        if (!ThenBranchExits(ifStatement.Statement))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0145, elseClause.ElseKeyword.GetLocation()));
    }

    private static bool ThenBranchExits(StatementSyntax statement)
    {
        var effective = statement;
        if (statement is BlockSyntax block)
        {
            if (block.Statements.Count == 0)
            {
                return false;
            }

            effective = block.Statements[block.Statements.Count - 1];
        }

        switch (effective.Kind())
        {
            case SyntaxKind.ReturnStatement:
            case SyntaxKind.ThrowStatement:
            case SyntaxKind.BreakStatement:
            case SyntaxKind.ContinueStatement:
            case SyntaxKind.GotoStatement:
                return true;
            default:
                return false;
        }
    }
}
