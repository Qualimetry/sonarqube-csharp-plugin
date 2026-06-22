using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StringFormatArgumentCountAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0096);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method)
        {
            return;
        }

        if (method.Name != "Format" || method.ContainingType?.SpecialType != SpecialType.System_String)
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count == 0)
        {
            return;
        }

        var formatIndex = method.Parameters.Length > 0 && method.Parameters[0].Type.Name == "IFormatProvider" ? 1 : 0;
        if (arguments.Count <= formatIndex)
        {
            return;
        }

        if (context.SemanticModel.GetConstantValue(arguments[formatIndex].Expression, context.CancellationToken) is not { HasValue: true, Value: string format })
        {
            return;
        }

        var valueArgumentCount = arguments.Count - (formatIndex + 1);
        if (valueArgumentCount == 1)
        {
            var argumentType = context.SemanticModel.GetTypeInfo(arguments[formatIndex + 1].Expression, context.CancellationToken).Type;
            if (argumentType is null || argumentType.TypeKind == TypeKind.Array)
            {
                return;
            }
        }

        var maxIndex = MaxPlaceholderIndex(format);
        if (maxIndex < 0)
        {
            return;
        }

        if (maxIndex >= valueArgumentCount)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0096, invocation.GetLocation()));
        }
    }

    private static int MaxPlaceholderIndex(string format)
    {
        var max = -1;

        for (var i = 0; i < format.Length; i++)
        {
            var c = format[i];

            if (c == '}')
            {
                if (i + 1 < format.Length && format[i + 1] == '}')
                {
                    i++;
                }

                continue;
            }

            if (c != '{')
            {
                continue;
            }

            if (i + 1 < format.Length && format[i + 1] == '{')
            {
                i++;
                continue;
            }

            var digitStart = i + 1;
            var j = digitStart;
            while (j < format.Length && char.IsDigit(format[j]))
            {
                j++;
            }

            if (j == digitStart)
            {
                return -1;
            }

            if (!int.TryParse(format.Substring(digitStart, j - digitStart), out var index))
            {
                return -1;
            }

            if (index > max)
            {
                max = index;
            }

            i = j - 1;
        }

        return max;
    }
}
