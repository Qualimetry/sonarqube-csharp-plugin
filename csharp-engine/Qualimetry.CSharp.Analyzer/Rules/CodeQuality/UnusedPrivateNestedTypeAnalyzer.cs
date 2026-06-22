using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedPrivateNestedTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0128);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.RecordStructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;

        if (!declaration.Modifiers.Any(SyntaxKind.PrivateKeyword)
            || declaration.Modifiers.Any(SyntaxKind.PartialKeyword)
            || declaration.AttributeLists.Count > 0)
        {
            return;
        }

        if (declaration.Parent is not TypeDeclarationSyntax containingType)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } symbol)
        {
            return;
        }

        if (IsReferenced(context, containingType, declaration, symbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0128, declaration.Identifier.GetLocation()));
    }

    private static bool IsReferenced(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax containingType, TypeDeclarationSyntax declaration, INamedTypeSymbol symbol)
    {
        foreach (var name in containingType.DescendantNodes().OfType<SimpleNameSyntax>())
        {
            if (name.Identifier.ValueText != symbol.Name)
            {
                continue;
            }

            if (declaration.Span.Contains(name.Span))
            {
                continue;
            }

            var referenced = context.SemanticModel.GetSymbolInfo(name, context.CancellationToken).Symbol;
            if (referenced is INamedTypeSymbol named
                && SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, symbol))
            {
                return true;
            }
        }

        return false;
    }
}
