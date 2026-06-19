using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PropertyChangedEventArgsAllocationAnalyzer : DiagnosticAnalyzer
{
    private const string TypeName = "System.ComponentModel.PropertyChangedEventArgs";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0134);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ObjectCreationExpression,
            SyntaxKind.ImplicitObjectCreationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var creation = (BaseObjectCreationExpressionSyntax)context.Node;

        if (context.SemanticModel.GetTypeInfo(creation).Type is not { } type
            || type.ToDisplayString() != TypeName)
        {
            return;
        }

        if (creation.Ancestors().Any(ancestor => ancestor is FieldDeclarationSyntax))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0134, creation.GetLocation()));
    }
}
