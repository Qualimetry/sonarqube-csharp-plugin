using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SqlCommandInjectionAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> CommandTypes = ImmutableHashSet.Create(
        "SqlCommand",
        "OleDbCommand",
        "OdbcCommand",
        "DbCommand",
        "MySqlCommand",
        "NpgsqlCommand",
        "SqliteCommand");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0148);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(AnalyzeCreation, SyntaxKind.ObjectCreationExpression);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        var targetName = assignment.Left switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            _ => null,
        };

        if (targetName != "CommandText")
        {
            return;
        }

        if (IsTaintedSql(assignment.Right))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0148, assignment.Right.GetLocation()));
        }
    }

    private static void AnalyzeCreation(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;

        if (!CommandTypes.Contains(SimpleTypeName(creation.Type) ?? string.Empty))
        {
            return;
        }

        var arguments = creation.ArgumentList?.Arguments;
        if (arguments is null || arguments.Value.Count == 0)
        {
            return;
        }

        var first = arguments.Value[0].Expression;
        if (IsTaintedSql(first))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0148, first.GetLocation()));
        }
    }

    internal static bool IsTaintedSql(ExpressionSyntax expression)
    {
        switch (Unwrap(expression))
        {
            case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.AddExpression):
                return BinaryHasNonConstantOperand(binary);
            case InterpolatedStringExpressionSyntax interpolated:
                return InterpolationHasNonConstant(interpolated);
            case InvocationExpressionSyntax invocation:
                return IsStringFormat(invocation);
            default:
                return false;
        }
    }

    private static bool BinaryHasNonConstantOperand(BinaryExpressionSyntax binary)
    {
        return ContainsNonConstant(binary.Left) || ContainsNonConstant(binary.Right);
    }

    private static bool ContainsNonConstant(ExpressionSyntax expression)
    {
        expression = Unwrap(expression);
        if (expression is BinaryExpressionSyntax binary && binary.IsKind(SyntaxKind.AddExpression))
        {
            return ContainsNonConstant(binary.Left) || ContainsNonConstant(binary.Right);
        }

        return !expression.IsKind(SyntaxKind.StringLiteralExpression);
    }

    private static bool InterpolationHasNonConstant(InterpolatedStringExpressionSyntax interpolated)
    {
        foreach (var content in interpolated.Contents)
        {
            if (content is InterpolationSyntax)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsStringFormat(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is MemberAccessExpressionSyntax memberAccess
            && memberAccess.Name.Identifier.ValueText == "Format"
            && memberAccess.Expression is IdentifierNameSyntax { Identifier.ValueText: "string" or "String" };
    }

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            expression = parenthesized.Expression;
        }

        return expression;
    }

    internal static string? SimpleTypeName(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            GenericNameSyntax generic => generic.Identifier.ValueText,
            _ => null,
        };
    }
}
