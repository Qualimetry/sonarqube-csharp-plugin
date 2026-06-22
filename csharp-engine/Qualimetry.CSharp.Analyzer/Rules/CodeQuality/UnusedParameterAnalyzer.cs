using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedParameterAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0168);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (method.AttributeLists.Count > 0
            || method.ExplicitInterfaceSpecifier is not null
            || method.Modifiers.Any(SyntaxKind.OverrideKeyword)
            || method.Modifiers.Any(SyntaxKind.VirtualKeyword)
            || method.Modifiers.Any(SyntaxKind.AbstractKeyword)
            || method.Modifiers.Any(SyntaxKind.ExternKeyword)
            || method.Modifiers.Any(SyntaxKind.PartialKeyword)
            || !method.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            return;
        }

        SyntaxNode? body = (SyntaxNode?)method.Body ?? method.ExpressionBody;
        if (body is null || method.ParameterList.Parameters.Count == 0)
        {
            return;
        }

        foreach (var parameter in method.ParameterList.Parameters)
        {
            if (parameter.Identifier.ValueText == "_" || parameter.AttributeLists.Count > 0)
            {
                continue;
            }

            if (context.SemanticModel.GetDeclaredSymbol(parameter, context.CancellationToken) is not { } symbol)
            {
                continue;
            }

            if (IsReferenced(context, body, symbol))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0168, parameter.Identifier.GetLocation()));
        }
    }

    private static bool IsReferenced(SyntaxNodeAnalysisContext context, SyntaxNode body, IParameterSymbol symbol)
    {
        foreach (var identifier in body.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (identifier.Identifier.ValueText != symbol.Name)
            {
                continue;
            }

            if (SymbolEqualityComparer.Default.Equals(context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol, symbol))
            {
                return true;
            }
        }

        return false;
    }
}
