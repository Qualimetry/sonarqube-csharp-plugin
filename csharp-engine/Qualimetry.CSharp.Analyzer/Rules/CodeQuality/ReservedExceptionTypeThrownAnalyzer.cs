using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ReservedExceptionTypeThrownAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> ReservedTypes = ImmutableHashSet.Create(
        "NullReferenceException",
        "IndexOutOfRangeException",
        "AccessViolationException",
        "OutOfMemoryException",
        "StackOverflowException",
        "ExecutionEngineException");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0066);

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

        var name = ExceptionTypeName.Of(creation.Type);
        if (name is null || !ReservedTypes.Contains(name))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0066, creation.GetLocation(), name));
    }
}
