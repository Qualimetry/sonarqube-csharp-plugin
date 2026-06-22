using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StringFormatInterpolationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0179);

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

        if (memberAccess.Name.Identifier.ValueText != "Format")
        {
            return;
        }

        if (!(context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol method) || !method.IsStatic)
        {
            return;
        }

        if (method.ContainingType?.SpecialType != SpecialType.System_String)
        {
            return;
        }

        var parameters = method.Parameters;
        if (parameters.Length > 0 && parameters[0].Type.ToDisplayString() == "System.IFormatProvider")
        {
            return;
        }

        foreach (var parameter in parameters)
        {
            var elementType = parameter.Type.OriginalDefinition.ToDisplayString();
            if (elementType == "System.Span<T>" || elementType == "System.ReadOnlySpan<T>")
            {
                return;
            }
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count == 0)
        {
            return;
        }

        var formatConstant = context.SemanticModel.GetConstantValue(arguments[0].Expression, context.CancellationToken);
        if (!formatConstant.HasValue || formatConstant.Value is not string)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0179, invocation.GetLocation()));
    }
}
