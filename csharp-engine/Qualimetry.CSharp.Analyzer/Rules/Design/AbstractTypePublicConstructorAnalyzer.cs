using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Design;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AbstractTypePublicConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0004);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ConstructorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var constructor = (ConstructorDeclarationSyntax)context.Node;
        if (!constructor.Modifiers.Any(SyntaxKind.PublicKeyword))
        {
            return;
        }

        if (constructor.Parent is not TypeDeclarationSyntax typeDeclaration)
        {
            return;
        }

        if (!typeDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword))
        {
            return;
        }

        var publicModifier = constructor.Modifiers.First(m => m.IsKind(SyntaxKind.PublicKeyword));
        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0004, publicModifier.GetLocation()));
    }
}
