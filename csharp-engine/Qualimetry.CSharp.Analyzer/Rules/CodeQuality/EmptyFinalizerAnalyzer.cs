using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EmptyFinalizerAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0144);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.DestructorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var destructor = (DestructorDeclarationSyntax)context.Node;

        if (destructor.ExpressionBody is not null)
        {
            return;
        }

        if (destructor.Body is not { Statements.Count: 0 })
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0144, destructor.Identifier.GetLocation()));
    }
}
