using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExpressionBodyOpportunityAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0191);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(
            AnalyzeAccessor,
            SyntaxKind.GetAccessorDeclaration,
            SyntaxKind.SetAccessorDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.ExpressionBody != null || !IsSingleConvertibleStatement(method.Body))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0191,
            method.Identifier.GetLocation()));
    }

    private static void AnalyzeAccessor(SyntaxNodeAnalysisContext context)
    {
        var accessor = (AccessorDeclarationSyntax)context.Node;
        if (accessor.ExpressionBody != null || !IsSingleConvertibleStatement(accessor.Body))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0191,
            accessor.Keyword.GetLocation()));
    }

    private static bool IsSingleConvertibleStatement(BlockSyntax? body)
    {
        if (body is null || body.Statements.Count != 1)
        {
            return false;
        }

        return body.Statements[0] switch
        {
            ReturnStatementSyntax { Expression: not null } => true,
            ExpressionStatementSyntax => true,
            _ => false,
        };
    }
}
