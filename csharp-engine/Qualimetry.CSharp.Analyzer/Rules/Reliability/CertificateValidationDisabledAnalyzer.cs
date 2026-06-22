using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CertificateValidationDisabledAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> CallbackNames = ImmutableHashSet.Create(
        "ServerCertificateCustomValidationCallback",
        "ServerCertificateValidationCallback",
        "RemoteCertificateValidationCallback");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0074);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleAssignmentExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        var targetName = assignment.Left switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            _ => null,
        };

        if (targetName is null || !CallbackNames.Contains(targetName))
        {
            return;
        }

        if (!AlwaysReturnsTrue(assignment.Right))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0074, assignment.Right.GetLocation()));
    }

    private static bool AlwaysReturnsTrue(ExpressionSyntax expression)
    {
        return expression switch
        {
            ParenthesizedLambdaExpressionSyntax lambda => BodyIsTrue(lambda.ExpressionBody, lambda.Block),
            SimpleLambdaExpressionSyntax lambda => BodyIsTrue(lambda.ExpressionBody, lambda.Block),
            _ => false,
        };
    }

    private static bool BodyIsTrue(ExpressionSyntax? expressionBody, BlockSyntax? block)
    {
        if (expressionBody is not null)
        {
            return expressionBody.IsKind(SyntaxKind.TrueLiteralExpression);
        }

        if (block is null)
        {
            return false;
        }

        var statements = block.Statements;
        return statements.Count == 1
            && statements[0] is ReturnStatementSyntax returnStatement
            && returnStatement.Expression is not null
            && returnStatement.Expression.IsKind(SyntaxKind.TrueLiteralExpression);
    }
}
