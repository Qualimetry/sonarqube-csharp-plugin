using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PublicReadonlyArrayFieldAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0135);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (!field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            return;
        }

        if (field.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return;
        }

        if (field.Declaration.Type is not ArrayTypeSyntax)
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not IFieldSymbol symbol)
            {
                continue;
            }

            if (symbol.DeclaredAccessibility is not (Accessibility.Public
                or Accessibility.Protected
                or Accessibility.Internal
                or Accessibility.ProtectedOrInternal))
            {
                continue;
            }

            if (!ContainingTypeIsExternallyVisible(symbol.ContainingType))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0135, variable.Identifier.GetLocation()));
        }
    }

    private static bool ContainingTypeIsExternallyVisible(INamedTypeSymbol? containingType)
    {
        for (INamedTypeSymbol? current = containingType; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is not (Accessibility.Public
                or Accessibility.Protected
                or Accessibility.ProtectedOrInternal))
            {
                return false;
            }
        }

        return true;
    }
}
