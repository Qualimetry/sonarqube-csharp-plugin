using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseNameOfForParameterNameAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0174);

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

        if (context.SemanticModel.GetSymbolInfo(creation, context.CancellationToken).Symbol is not IMethodSymbol constructor
            || !IsArgumentException(constructor.ContainingType))
        {
            return;
        }

        var parameterNames = CollectEnclosingParameterNames(creation);
        if (parameterNames.Count == 0)
        {
            return;
        }

        foreach (var argument in creation.ArgumentList.Arguments)
        {
            if (argument.Expression is LiteralExpressionSyntax literal
                && literal.IsKind(SyntaxKind.StringLiteralExpression)
                && parameterNames.Contains(literal.Token.ValueText))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0174, literal.GetLocation()));
            }
        }
    }

    private static bool IsArgumentException(INamedTypeSymbol? type)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (current.Name == "ArgumentException" && current.ContainingNamespace?.ToDisplayString() == "System")
            {
                return true;
            }
        }

        return false;
    }

    private static HashSet<string> CollectEnclosingParameterNames(SyntaxNode node)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);

        for (SyntaxNode? current = node; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case BaseMethodDeclarationSyntax method:
                    AddParameters(names, method.ParameterList);
                    break;
                case LocalFunctionStatementSyntax local:
                    AddParameters(names, local.ParameterList);
                    break;
                case ParenthesizedLambdaExpressionSyntax lambda:
                    AddParameters(names, lambda.ParameterList);
                    break;
                case SimpleLambdaExpressionSyntax simple:
                    names.Add(simple.Parameter.Identifier.ValueText);
                    break;
                case AnonymousMethodExpressionSyntax anonymous when anonymous.ParameterList is not null:
                    AddParameters(names, anonymous.ParameterList);
                    break;
            }
        }

        return names;
    }

    private static void AddParameters(HashSet<string> names, ParameterListSyntax parameterList)
    {
        foreach (var parameter in parameterList.Parameters)
        {
            names.Add(parameter.Identifier.ValueText);
        }
    }
}
