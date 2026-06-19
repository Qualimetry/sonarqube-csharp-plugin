using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PropertyGetterMutatesStateAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0133);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.GetAccessorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var accessor = (AccessorDeclarationSyntax)context.Node;

        SyntaxNode? body = (SyntaxNode?)accessor.Body ?? accessor.ExpressionBody;
        if (body is null)
        {
            return;
        }

        foreach (var node in body.DescendantNodes())
        {
            switch (node)
            {
                case PostfixUnaryExpressionSyntax postfix
                    when postfix.IsKind(SyntaxKind.PostIncrementExpression) || postfix.IsKind(SyntaxKind.PostDecrementExpression):
                    if (MutatesMemberState(context, postfix.Operand))
                    {
                        Report(context, accessor);
                        return;
                    }

                    break;
                case PrefixUnaryExpressionSyntax prefix
                    when prefix.IsKind(SyntaxKind.PreIncrementExpression) || prefix.IsKind(SyntaxKind.PreDecrementExpression):
                    if (MutatesMemberState(context, prefix.Operand))
                    {
                        Report(context, accessor);
                        return;
                    }

                    break;
                case AssignmentExpressionSyntax assignment
                    when !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression):
                    if (MutatesMemberState(context, assignment.Left))
                    {
                        Report(context, accessor);
                        return;
                    }

                    break;
            }
        }
    }

    private static bool MutatesMemberState(SyntaxNodeAnalysisContext context, ExpressionSyntax target)
    {
        var symbol = context.SemanticModel.GetSymbolInfo(target, context.CancellationToken).Symbol;
        return symbol is IFieldSymbol or IPropertySymbol or IEventSymbol;
    }

    private static void Report(SyntaxNodeAnalysisContext context, AccessorDeclarationSyntax accessor)
    {
        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0133, accessor.Keyword.GetLocation()));
    }
}
