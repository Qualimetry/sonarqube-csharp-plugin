using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InitializeOnLoadStaticConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0049);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (!UnityAttributes.Has(declaration.AttributeLists, "InitializeOnLoad"))
        {
            return;
        }

        var hasStaticConstructor = declaration.Members
            .OfType<ConstructorDeclarationSyntax>()
            .Any(ctor => ctor.Modifiers.Any(SyntaxKind.StaticKeyword));

        if (hasStaticConstructor)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0049, declaration.Identifier.GetLocation()));
    }
}
