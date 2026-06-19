using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ConsoleCompositeFormatAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0178);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (!(invocation.Expression is MemberAccessExpressionSyntax memberAccess))
        {
            return;
        }

        var methodName = memberAccess.Name.Identifier.ValueText;
        if (methodName != "WriteLine" && methodName != "Write")
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count < 2)
        {
            return;
        }

        if (!(arguments[0].Expression is LiteralExpressionSyntax literal) || !literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return;
        }

        if (!ContainsPlaceholder(literal.Token.ValueText))
        {
            return;
        }

        if (!(context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol method)
            || method.ContainingType?.ToDisplayString() != "System.Console")
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0178, invocation.GetLocation()));
    }

    private static bool ContainsPlaceholder(string value)
    {
        for (var i = 0; i + 1 < value.Length; i++)
        {
            if (value[i] == '{' && char.IsDigit(value[i + 1]))
            {
                return true;
            }
        }

        return false;
    }
}
