using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BoxingConversionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0201);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterOperationAction(Analyze, OperationKind.Conversion);
    }

    private static void Analyze(OperationAnalysisContext context)
    {
        if (context.Operation is not IConversionOperation conversion || !conversion.IsImplicit)
        {
            return;
        }

        var sourceType = conversion.Operand?.Type;
        var targetType = conversion.Type;
        if (sourceType == null || targetType == null)
        {
            return;
        }

        if (sourceType.IsValueType && targetType.IsReferenceType)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0201, conversion.Syntax.GetLocation()));
            return;
        }

        if (sourceType.SpecialType == SpecialType.System_Object && targetType.IsValueType)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0201, conversion.Syntax.GetLocation()));
        }
    }
}
