using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CustomDelegateDeclarationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0009);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.DelegateDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (DelegateDeclarationSyntax)context.Node;

        foreach (var parameter in declaration.ParameterList.Parameters)
        {
            if (parameter.Modifiers.Any(SyntaxKind.RefKeyword)
                || parameter.Modifiers.Any(SyntaxKind.OutKeyword)
                || parameter.Modifiers.Any(SyntaxKind.InKeyword)
                || parameter.Modifiers.Any(SyntaxKind.ParamsKeyword))
            {
                return;
            }
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0009, declaration.Identifier.GetLocation()));
    }
}
