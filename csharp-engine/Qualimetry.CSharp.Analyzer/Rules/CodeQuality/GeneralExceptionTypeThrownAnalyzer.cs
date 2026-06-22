using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class GeneralExceptionTypeThrownAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> GeneralTypes = ImmutableHashSet.Create(
        "Exception",
        "SystemException",
        "ApplicationException");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0067);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ThrowStatement, SyntaxKind.ThrowExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var thrown = context.Node switch
        {
            ThrowStatementSyntax statement => statement.Expression,
            ThrowExpressionSyntax expression => expression.Expression,
            _ => null,
        };

        if (thrown is not ObjectCreationExpressionSyntax creation)
        {
            return;
        }

        var type = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;
        if (type is null
            || type.ContainingNamespace?.ToDisplayString() != "System"
            || !GeneralTypes.Contains(type.Name))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0067, creation.GetLocation(), type.Name));
    }
}
