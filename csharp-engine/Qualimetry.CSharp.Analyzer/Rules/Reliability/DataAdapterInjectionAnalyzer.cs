using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DataAdapterInjectionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0149);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;

        var createdType = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;
        if (!ResolvesToDataAdapter(createdType))
        {
            return;
        }

        var arguments = creation.ArgumentList?.Arguments;
        if (arguments is null || arguments.Value.Count == 0)
        {
            return;
        }

        var first = arguments.Value[0].Expression;
        if (SqlCommandInjectionAnalyzer.IsTaintedSql(context.SemanticModel, first))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0149, first.GetLocation()));
        }
    }

    private static bool ResolvesToDataAdapter(ITypeSymbol? type)
    {
        for (var current = type as INamedTypeSymbol; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == "System.Data.Common.DataAdapter")
            {
                return true;
            }
        }

        return false;
    }
}
