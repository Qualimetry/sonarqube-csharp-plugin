using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LegacyNonGenericCollectionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0078);

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
            if (IsLegacyCollection(type.Name) && type.ContainingNamespace?.ToDisplayString() == "System.Collections")
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0078, creation.GetLocation()));
            }

            return;
        }

        if (creation.Type is IdentifierNameSyntax identifier && IsLegacyCollection(identifier.Identifier.ValueText))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0078, creation.GetLocation()));
        }
    }

    private static bool IsLegacyCollection(string name) =>
        name is "ArrayList" or "Hashtable";
}
