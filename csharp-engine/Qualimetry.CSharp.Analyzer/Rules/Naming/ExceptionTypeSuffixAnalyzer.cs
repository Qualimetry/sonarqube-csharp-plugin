using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExceptionTypeSuffixAnalyzer : DiagnosticAnalyzer
{
    private const string Suffix = "Exception";
    private const string ExceptionBaseType = "System.Exception";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0089);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol)
        {
            return;
        }

        if (!InheritsFrom(symbol, ExceptionBaseType))
        {
            return;
        }

        var name = declaration.Identifier.ValueText;
        if (name.EndsWith(Suffix, System.StringComparison.Ordinal))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0089, declaration.Identifier.GetLocation(), name));
    }

    private static bool InheritsFrom(INamedTypeSymbol symbol, string baseTypeDisplay)
    {
        for (var current = symbol.BaseType; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == baseTypeDisplay)
            {
                return true;
            }
        }

        return false;
    }
}
