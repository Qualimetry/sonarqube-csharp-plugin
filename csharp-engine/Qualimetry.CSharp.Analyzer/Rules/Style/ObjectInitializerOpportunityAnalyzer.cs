using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObjectInitializerOpportunityAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0175);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.LocalDeclarationStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (LocalDeclarationStatementSyntax)context.Node;
        if (declaration.Declaration.Variables.Count != 1)
        {
            return;
        }

        var variable = declaration.Declaration.Variables[0];
        if (variable.Initializer == null || !(variable.Initializer.Value is ObjectCreationExpressionSyntax creation))
        {
            return;
        }

        if (creation.Initializer != null)
        {
            return;
        }

        if (!(declaration.Parent is BlockSyntax block))
        {
            return;
        }

        var index = block.Statements.IndexOf(declaration);
        if (index < 0 || index + 1 >= block.Statements.Count)
        {
            return;
        }

        if (!IsPropertyAssignmentTo(block.Statements[index + 1], variable.Identifier.ValueText))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0175, creation.GetLocation()));
    }

    private static bool IsPropertyAssignmentTo(StatementSyntax statement, string variableName)
    {
        if (!(statement is ExpressionStatementSyntax expressionStatement))
        {
            return false;
        }

        if (!(expressionStatement.Expression is AssignmentExpressionSyntax assignment) || !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
        {
            return false;
        }

        return assignment.Left is MemberAccessExpressionSyntax memberAccess
            && memberAccess.Expression is IdentifierNameSyntax target
            && target.Identifier.ValueText == variableName;
    }
}
