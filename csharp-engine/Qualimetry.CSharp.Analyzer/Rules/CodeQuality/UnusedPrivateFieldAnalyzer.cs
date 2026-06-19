using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedPrivateFieldAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0126);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (field.Modifiers.Any(SyntaxKind.ConstKeyword)
            || field.AttributeLists.Count > 0)
        {
            return;
        }

        if (field.Parent is not TypeDeclarationSyntax containingType)
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not IFieldSymbol symbol)
            {
                continue;
            }

            if (symbol.DeclaredAccessibility != Accessibility.Private
                || symbol.ContainingType.DeclaringSyntaxReferences.Length > 1)
            {
                continue;
            }

            if (IsReferenced(context, containingType, symbol))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0126, variable.Identifier.GetLocation()));
        }
    }

    private static bool IsReferenced(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax containingType, IFieldSymbol symbol)
    {
        foreach (var identifier in containingType.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (identifier.Identifier.ValueText != symbol.Name)
            {
                continue;
            }

            var referenced = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol;
            if (SymbolEqualityComparer.Default.Equals(referenced, symbol))
            {
                return true;
            }
        }

        return false;
    }
}
