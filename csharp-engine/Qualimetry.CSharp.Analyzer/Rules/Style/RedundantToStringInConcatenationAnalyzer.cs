using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RedundantToStringInConcatenationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0164);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (invocation.ArgumentList.Arguments.Count != 0)
        {
            return;
        }

        if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess))
        {
            return;
        }

        if (memberAccess.Name.Identifier.ValueText != "ToString")
        {
            return;
        }

        if (!(invocation.Parent is BinaryExpressionSyntax binary) || !binary.IsKind(SyntaxKind.AddExpression))
        {
            return;
        }

        var binaryType = context.SemanticModel.GetTypeInfo(binary, context.CancellationToken).Type;
        if (binaryType == null || binaryType.SpecialType != SpecialType.System_String)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0164, invocation.GetLocation()));
    }
}
