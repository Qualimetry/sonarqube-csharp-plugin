using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RedundantLambdaMethodGroupAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0188);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.SimpleLambdaExpression,
            SyntaxKind.ParenthesizedLambdaExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var lambda = (LambdaExpressionSyntax)context.Node;

        if (GetForwardableParameters(lambda) is not { } parameters)
        {
            return;
        }

        if (lambda.Body is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count != parameters.Count)
        {
            return;
        }

        for (var i = 0; i < arguments.Count; i++)
        {
            var argument = arguments[i];
            if (argument.NameColon != null || !argument.RefKindKeyword.IsKind(SyntaxKind.None))
            {
                return;
            }

            if (argument.Expression is not IdentifierNameSyntax identifier
                || identifier.Identifier.ValueText != parameters[i].Identifier.ValueText)
            {
                return;
            }
        }

        if (CalleeReferencesAnyParameter(invocation.Expression, parameters))
        {
            return;
        }

        var model = context.SemanticModel;

        if (IsExpressionTreeTarget(model, lambda))
        {
            return;
        }

        if (model.GetSymbolInfo(invocation).Symbol is not IMethodSymbol method)
        {
            return;
        }

        if (method.IsExtensionMethod || method.IsGenericMethod || method.Parameters.Length != arguments.Count)
        {
            return;
        }

        for (var i = 0; i < parameters.Count; i++)
        {
            if (model.GetDeclaredSymbol(parameters[i]) is not IParameterSymbol parameterSymbol)
            {
                return;
            }

            if (!SymbolEqualityComparer.Default.Equals(parameterSymbol.Type, method.Parameters[i].Type))
            {
                return;
            }
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0188, lambda.GetLocation()));
    }

    private static SeparatedSyntaxList<ParameterSyntax>? GetForwardableParameters(LambdaExpressionSyntax lambda)
    {
        switch (lambda)
        {
            case SimpleLambdaExpressionSyntax simple:
                return simple.Parameter.Modifiers.Count != 0
                    ? null
                    : SyntaxFactory.SingletonSeparatedList(simple.Parameter);
            case ParenthesizedLambdaExpressionSyntax parenthesized:
                foreach (var parameter in parenthesized.ParameterList.Parameters)
                {
                    if (parameter.Modifiers.Count != 0)
                    {
                        return null;
                    }
                }

                return parenthesized.ParameterList.Parameters;
            default:
                return null;
        }
    }

    private static bool CalleeReferencesAnyParameter(ExpressionSyntax callee, SeparatedSyntaxList<ParameterSyntax> parameters)
    {
        foreach (var identifier in callee.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>())
        {
            foreach (var parameter in parameters)
            {
                if (identifier.Identifier.ValueText == parameter.Identifier.ValueText)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsExpressionTreeTarget(SemanticModel model, LambdaExpressionSyntax lambda)
    {
        for (var type = model.GetTypeInfo(lambda).ConvertedType; type != null; type = type.BaseType)
        {
            if (type.ToDisplayString().StartsWith("System.Linq.Expressions.Expression", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
