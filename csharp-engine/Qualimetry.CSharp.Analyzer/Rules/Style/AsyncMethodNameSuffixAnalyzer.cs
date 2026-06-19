using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncMethodNameSuffixAnalyzer : DiagnosticAnalyzer
{
    private const string Suffix = "Async";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0006);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!method.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            return;
        }

        var name = method.Identifier.ValueText;
        if (name.EndsWith(Suffix, System.StringComparison.Ordinal))
        {
            return;
        }

        if (name == "Main")
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method) is { } symbol
            && (symbol.IsOverride || !symbol.ExplicitInterfaceImplementations.IsEmpty))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0006, method.Identifier.GetLocation()));
    }
}
