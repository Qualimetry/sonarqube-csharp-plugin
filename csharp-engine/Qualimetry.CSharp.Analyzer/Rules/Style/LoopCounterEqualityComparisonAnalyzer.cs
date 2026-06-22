using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LoopCounterEqualityComparisonAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0102);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ForStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var forStatement = (ForStatementSyntax)context.Node;
        if (forStatement.Incrementors.Count != 1)
        {
            return;
        }

        if (forStatement.Condition is not BinaryExpressionSyntax condition)
        {
            return;
        }

        if (!condition.IsKind(SyntaxKind.NotEqualsExpression) && !condition.IsKind(SyntaxKind.EqualsExpression))
        {
            return;
        }

        if (!TryGetUnitStepCounter(forStatement.Incrementors[0], out var counter))
        {
            return;
        }

        var counterOperand = MatchCounterOperand(condition, counter);
        if (counterOperand == null)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(counterOperand, context.CancellationToken).Type;
        if (type == null || !IsNumeric(type))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0102, condition.GetLocation()));
    }

    private static bool TryGetUnitStepCounter(ExpressionSyntax incrementor, out string counter)
    {
        switch (incrementor)
        {
            case PostfixUnaryExpressionSyntax postfix
                when postfix.IsKind(SyntaxKind.PostIncrementExpression) || postfix.IsKind(SyntaxKind.PostDecrementExpression):
                return TryGetName(postfix.Operand, out counter);
            case PrefixUnaryExpressionSyntax prefix
                when prefix.IsKind(SyntaxKind.PreIncrementExpression) || prefix.IsKind(SyntaxKind.PreDecrementExpression):
                return TryGetName(prefix.Operand, out counter);
            case AssignmentExpressionSyntax assignment
                when (assignment.IsKind(SyntaxKind.AddAssignmentExpression) || assignment.IsKind(SyntaxKind.SubtractAssignmentExpression))
                    && IsOneLiteral(assignment.Right):
                return TryGetName(assignment.Left, out counter);
            default:
                counter = string.Empty;
                return false;
        }
    }

    private static bool TryGetName(ExpressionSyntax expression, out string name)
    {
        if (expression is IdentifierNameSyntax identifier)
        {
            name = identifier.Identifier.ValueText;
            return true;
        }

        name = string.Empty;
        return false;
    }

    private static bool IsOneLiteral(ExpressionSyntax expression) =>
        expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.NumericLiteralExpression)
            && literal.Token.ValueText == "1";

    private static ExpressionSyntax? MatchCounterOperand(BinaryExpressionSyntax condition, string counter)
    {
        if (condition.Left is IdentifierNameSyntax left && left.Identifier.ValueText == counter)
        {
            return condition.Left;
        }

        if (condition.Right is IdentifierNameSyntax right && right.Identifier.ValueText == counter)
        {
            return condition.Right;
        }

        return null;
    }

    private static bool IsNumeric(ITypeSymbol type) =>
        type.SpecialType switch
        {
            SpecialType.System_SByte
                or SpecialType.System_Byte
                or SpecialType.System_Int16
                or SpecialType.System_UInt16
                or SpecialType.System_Int32
                or SpecialType.System_UInt32
                or SpecialType.System_Int64
                or SpecialType.System_UInt64
                or SpecialType.System_Single
                or SpecialType.System_Double
                or SpecialType.System_Decimal => true,
            _ => false,
        };
}
