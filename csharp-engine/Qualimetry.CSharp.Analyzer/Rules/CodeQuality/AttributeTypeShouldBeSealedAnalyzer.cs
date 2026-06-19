using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeTypeShouldBeSealedAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0008);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (declaration.Modifiers.Any(SyntaxKind.SealedKeyword)
            || declaration.Modifiers.Any(SyntaxKind.AbstractKeyword)
            || declaration.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        if (declaration.BaseList is null)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } typeSymbol)
        {
            return;
        }

        if (!DerivesFromAttribute(typeSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0008, declaration.Identifier.GetLocation()));
    }

    private static bool DerivesFromAttribute(INamedTypeSymbol typeSymbol)
    {
        for (INamedTypeSymbol? current = typeSymbol.BaseType; current is not null; current = current.BaseType)
        {
            if (current.Name == "Attribute" && current.ContainingNamespace?.ToDisplayString() == "System")
            {
                return true;
            }
        }

        return false;
    }
}
