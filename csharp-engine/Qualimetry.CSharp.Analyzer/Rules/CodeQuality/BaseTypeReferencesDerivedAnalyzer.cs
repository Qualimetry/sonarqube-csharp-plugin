using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BaseTypeReferencesDerivedAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0037);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;
        var model = context.SemanticModel;

        if (model.GetDeclaredSymbol(declaration) is not { } classSymbol)
        {
            return;
        }

        foreach (var node in declaration.DescendantNodes())
        {
            if (node is not (IdentifierNameSyntax or GenericNameSyntax))
            {
                continue;
            }

            if (node.Parent is BaseTypeSyntax)
            {
                continue;
            }

            if (model.GetSymbolInfo(node).Symbol is not INamedTypeSymbol referenced)
            {
                continue;
            }

            if (SymbolEqualityComparer.Default.Equals(referenced, classSymbol))
            {
                continue;
            }

            if (!InheritsFrom(referenced, classSymbol))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0037, node.GetLocation()));
            return;
        }
    }

    private static bool InheritsFrom(INamedTypeSymbol type, INamedTypeSymbol candidateBase)
    {
        for (var current = type.BaseType; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, candidateBase))
            {
                return true;
            }
        }

        return false;
    }
}
