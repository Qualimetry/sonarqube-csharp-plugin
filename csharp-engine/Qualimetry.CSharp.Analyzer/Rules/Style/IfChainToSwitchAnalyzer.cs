using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class IfChainToSwitchAnalyzer : DiagnosticAnalyzer
{
    private const int MinimumBranches = 3;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0180);

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

        string? variableName = null;
        var count = 0;
        var current = ifStatement;
        while (current != null)
        {
            if (!IsEqualityAgainstConstant(current.Condition, ref variableName))
            {
                return;
            }

            count++;
            var elseClause = current.Else;
            if (elseClause?.Statement is IfStatementSyntax nextIf)
            {
                current = nextIf;
            }
            else
            {
                break;
            }
        }

        if (count >= MinimumBranches)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0180, ifStatement.IfKeyword.GetLocation()));
        }
    }

    private static bool IsEqualityAgainstConstant(ExpressionSyntax condition, ref string? variableName)
    {
        if (!(condition is BinaryExpressionSyntax binary) || !binary.IsKind(SyntaxKind.EqualsExpression))
        {
            return false;
        }

        string? identifier = null;
        if (binary.Left is IdentifierNameSyntax left && IsConstantOperand(binary.Right))
        {
            identifier = left.Identifier.ValueText;
        }
        else if (binary.Right is IdentifierNameSyntax right && IsConstantOperand(binary.Left))
        {
            identifier = right.Identifier.ValueText;
        }

        if (identifier == null)
        {
            return false;
        }

        if (variableName == null)
        {
            variableName = identifier;
            return true;
        }

        return variableName == identifier;
    }

    private static bool IsConstantOperand(ExpressionSyntax expression)
    {
        switch (expression.Kind())
        {
            case SyntaxKind.NumericLiteralExpression:
            case SyntaxKind.StringLiteralExpression:
            case SyntaxKind.CharacterLiteralExpression:
            case SyntaxKind.TrueLiteralExpression:
            case SyntaxKind.FalseLiteralExpression:
                return true;
            default:
                return false;
        }
    }
}
