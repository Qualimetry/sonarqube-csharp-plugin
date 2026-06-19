using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MergeableNestedIfAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0107);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IfStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var outer = (IfStatementSyntax)context.Node;
        if (outer.Else != null)
        {
            return;
        }

        if (outer.Parent is ElseClauseSyntax)
        {
            return;
        }

        var inner = UnwrapSingleIf(outer.Statement);
        if (inner == null || inner.Else != null)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0107, outer.IfKeyword.GetLocation()));
    }

    private static IfStatementSyntax? UnwrapSingleIf(StatementSyntax statement)
    {
        switch (statement)
        {
            case IfStatementSyntax directIf:
                return directIf;
            case BlockSyntax block when block.Statements.Count == 1 && block.Statements[0] is IfStatementSyntax blockIf:
                return blockIf;
            default:
                return null;
        }
    }
}
