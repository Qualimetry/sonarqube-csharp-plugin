using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CloneableImplementationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0075);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.RecordStructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;

        if (declaration.BaseList is null)
        {
            return;
        }

        foreach (var baseType in declaration.BaseList.Types)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(baseType.Type, context.CancellationToken).Symbol;
            if (symbol is INamedTypeSymbol { Name: "ICloneable", ContainingNamespace: { } ns }
                && ns.ToDisplayString() == "System")
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0075, declaration.Identifier.GetLocation()));
                return;
            }

            if (symbol is null
                && baseType.Type is IdentifierNameSyntax { Identifier.ValueText: "ICloneable" })
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0075, declaration.Identifier.GetLocation()));
                return;
            }
        }
    }
}
