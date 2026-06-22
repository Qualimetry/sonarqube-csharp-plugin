using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferStringEmptyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0055);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StringLiteralExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var literal = (LiteralExpressionSyntax)context.Node;
        if (literal.Token.ValueText.Length != 0)
        {
            return;
        }

        if (RequiresCompileTimeConstant(literal))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0055, literal.GetLocation()));
    }

    private static bool RequiresCompileTimeConstant(SyntaxNode node)
    {
        for (SyntaxNode? current = node; current != null; current = current.Parent)
        {
            switch (current)
            {
                case AttributeSyntax:
                case ParameterSyntax:
                case CaseSwitchLabelSyntax:
                case ConstantPatternSyntax:
                    return true;
                case FieldDeclarationSyntax field when field.Modifiers.Any(SyntaxKind.ConstKeyword):
                    return true;
                case LocalDeclarationStatementSyntax local when local.IsConst:
                    return true;
                case MemberDeclarationSyntax:
                case StatementSyntax:
                    return false;
            }
        }

        return false;
    }
}
