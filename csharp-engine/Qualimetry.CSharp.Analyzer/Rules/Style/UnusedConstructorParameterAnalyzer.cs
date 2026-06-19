using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedConstructorParameterAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0053);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ConstructorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var constructor = (ConstructorDeclarationSyntax)context.Node;
        if (constructor.Body == null && constructor.ExpressionBody == null)
        {
            return;
        }

        if (constructor.Modifiers.Any(SyntaxKind.ExternKeyword))
        {
            return;
        }

        var parameters = constructor.ParameterList.Parameters;
        if (parameters.Count == 0)
        {
            return;
        }

        var referenced = constructor
            .DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Select(id => context.SemanticModel.GetSymbolInfo(id).Symbol)
            .Where(symbol => symbol is IParameterSymbol)
            .ToImmutableHashSet(SymbolEqualityComparer.Default);

        foreach (var parameter in parameters)
        {
            if (parameter.Identifier.ValueText == "_")
            {
                continue;
            }

            if (context.SemanticModel.GetDeclaredSymbol(parameter) is not { } parameterSymbol)
            {
                continue;
            }

            if (referenced.Contains(parameterSymbol))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0053,
                parameter.Identifier.GetLocation(),
                parameter.Identifier.ValueText));
        }
    }
}
