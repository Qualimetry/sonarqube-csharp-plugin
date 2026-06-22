using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ObsoleteMemberUsageAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0082);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IdentifierName, SyntaxKind.GenericName);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var name = (SimpleNameSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(name, context.CancellationToken).Symbol is not { } symbol)
        {
            return;
        }

        if (!IsConsideredSymbol(symbol))
        {
            return;
        }

        if (!CarriesObsoleteAttribute(symbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0082, name.GetLocation()));
    }

    private static bool IsConsideredSymbol(ISymbol symbol)
    {
        return symbol.Kind switch
        {
            SymbolKind.Method => true,
            SymbolKind.Property => true,
            SymbolKind.Field => true,
            SymbolKind.Event => true,
            SymbolKind.NamedType => true,
            _ => false,
        };
    }

    private static bool CarriesObsoleteAttribute(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "ObsoleteAttribute");
    }
}
