using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedPrivateMethodAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0127);

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
            || method.Modifiers.Any(SyntaxKind.PartialKeyword)
            || method.Modifiers.Any(SyntaxKind.ExternKeyword))
        {
            return;
        }

        if (method.Parent is not TypeDeclarationSyntax containingType)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is not { } symbol
            || symbol.DeclaredAccessibility != Accessibility.Private
            || symbol.ContainingType.DeclaringSyntaxReferences.Length > 1)
        {
            return;
        }

        if (IsReferenced(context, containingType, symbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0127, method.Identifier.GetLocation()));
    }

    private static bool IsReferenced(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax containingType, IMethodSymbol symbol)
    {
        foreach (var identifier in containingType.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (identifier.Identifier.ValueText != symbol.Name)
            {
                continue;
            }

            var info = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken);
            if (SymbolEqualityComparer.Default.Equals(info.Symbol, symbol)
                || info.CandidateSymbols.Any(candidate => SymbolEqualityComparer.Default.Equals(candidate, symbol)))
            {
                return true;
            }
        }

        return false;
    }
}
