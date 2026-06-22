using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ArgumentExceptionParameterNameAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0101);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;

        if (creation.ArgumentList is null || creation.ArgumentList.Arguments.Count == 0)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(creation, context.CancellationToken).Symbol is not IMethodSymbol constructor)
        {
            return;
        }

        if (!DerivesFromArgumentException(constructor.ContainingType))
        {
            return;
        }

        var paramIndex = -1;
        for (var i = 0; i < constructor.Parameters.Length; i++)
        {
            if (constructor.Parameters[i].Name == "paramName")
            {
                paramIndex = i;
                break;
            }
        }

        if (paramIndex < 0 || paramIndex >= creation.ArgumentList.Arguments.Count)
        {
            return;
        }

        var argument = creation.ArgumentList.Arguments[paramIndex].Expression;
        var providedName = ExtractName(argument);
        if (providedName is null)
        {
            return;
        }

        var parameterNames = CollectEnclosingParameterNames(creation);
        if (parameterNames.Contains(providedName))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0101, argument.GetLocation()));
    }

    private static bool DerivesFromArgumentException(INamedTypeSymbol? type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (current.Name == "ArgumentException" && current.ContainingNamespace?.ToDisplayString() == "System")
            {
                return true;
            }
        }

        return false;
    }

    private static string? ExtractName(ExpressionSyntax expression)
    {
        if (expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return literal.Token.ValueText;
        }

        if (expression is InvocationExpressionSyntax invocation
            && invocation.Expression is IdentifierNameSyntax { Identifier.ValueText: "nameof" }
            && invocation.ArgumentList.Arguments.Count == 1)
        {
            return invocation.ArgumentList.Arguments[0].Expression switch
            {
                IdentifierNameSyntax id => id.Identifier.ValueText,
                MemberAccessExpressionSyntax member => member.Name.Identifier.ValueText,
                _ => null,
            };
        }

        return null;
    }

    private static HashSet<string> CollectEnclosingParameterNames(SyntaxNode node)
    {
        var names = new HashSet<string>();

        for (var current = node.Parent; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case BaseMethodDeclarationSyntax method:
                    AddParameters(method.ParameterList, names);
                    break;
                case LocalFunctionStatementSyntax local:
                    AddParameters(local.ParameterList, names);
                    break;
                case IndexerDeclarationSyntax indexer:
                    AddParameters(indexer.ParameterList, names);
                    break;
                case ParenthesizedLambdaExpressionSyntax lambda:
                    AddParameters(lambda.ParameterList, names);
                    break;
                case SimpleLambdaExpressionSyntax simpleLambda:
                    names.Add(simpleLambda.Parameter.Identifier.ValueText);
                    break;
                case AccessorDeclarationSyntax accessor
                    when accessor.IsKind(SyntaxKind.SetAccessorDeclaration) || accessor.IsKind(SyntaxKind.InitAccessorDeclaration):
                    names.Add("value");
                    break;
            }
        }

        return names;
    }

    private static void AddParameters(BaseParameterListSyntax? parameterList, HashSet<string> names)
    {
        if (parameterList is null)
        {
            return;
        }

        foreach (var parameter in parameterList.Parameters)
        {
            names.Add(parameter.Identifier.ValueText);
        }
    }
}
