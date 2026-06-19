using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HiddenBaseMethodAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0064);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!method.Modifiers.Any(SyntaxKind.NewKeyword))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method) is not { } methodSymbol)
        {
            return;
        }

        if (!BaseTypeDeclaresMethod(methodSymbol.ContainingType, methodSymbol.Name))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0064,
            method.Identifier.GetLocation(),
            methodSymbol.Name));
    }

    private static bool BaseTypeDeclaresMethod(INamedTypeSymbol containingType, string name)
    {
        for (var current = containingType.BaseType; current is not null; current = current.BaseType)
        {
            if (current.GetMembers(name).OfType<IMethodSymbol>().Any(m => m.MethodKind == MethodKind.Ordinary))
            {
                return true;
            }
        }

        return false;
    }
}
