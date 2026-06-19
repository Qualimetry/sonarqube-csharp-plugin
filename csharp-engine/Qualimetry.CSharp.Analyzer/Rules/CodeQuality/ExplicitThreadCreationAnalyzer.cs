using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExplicitThreadCreationAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0073);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;

        var type = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;
        if (type is not null)
        {
            if (type.Name == "Thread" && type.ContainingNamespace?.ToDisplayString() == "System.Threading")
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0073, creation.GetLocation()));
            }

            return;
        }

        if (creation.Type is IdentifierNameSyntax { Identifier.ValueText: "Thread" })
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0073, creation.GetLocation()));
        }
    }
}
