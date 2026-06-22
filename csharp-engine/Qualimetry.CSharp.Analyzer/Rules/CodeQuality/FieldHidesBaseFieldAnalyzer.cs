using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FieldHidesBaseFieldAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0015);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (field.Modifiers.Any(SyntaxKind.NewKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not IFieldSymbol symbol)
            {
                continue;
            }

            if (BaseTypeDeclaresField(symbol.ContainingType, symbol.Name))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0015,
                    variable.Identifier.GetLocation(),
                    symbol.Name));
            }
        }
    }

    private static bool BaseTypeDeclaresField(INamedTypeSymbol? containingType, string name)
    {
        if (containingType is null)
        {
            return false;
        }

        for (INamedTypeSymbol? current = containingType.BaseType; current is not null; current = current.BaseType)
        {
            if (current.GetMembers(name).OfType<IFieldSymbol>().Any(f => !f.IsImplicitlyDeclared))
            {
                return true;
            }
        }

        return false;
    }
}
