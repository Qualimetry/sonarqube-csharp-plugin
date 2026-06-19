using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MutableComplexFieldAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxMutableComplexFields = 0;
    private const string Option = "qualimetry.qa_metrics_mutable_complex_field.maxmutablecomplexfields";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0052);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;
        var threshold = OptionReader.ReadInt(context, declaration.SyntaxTree, Option, DefaultMaxMutableComplexFields);

        var count = 0;
        foreach (var member in declaration.Members)
        {
            if (member is not FieldDeclarationSyntax field)
            {
                continue;
            }

            if (field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
                || field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                continue;
            }

            if (field.Declaration.Type is PredefinedTypeSyntax)
            {
                continue;
            }

            count += field.Declaration.Variables.Count;
        }

        if (count > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0052,
                declaration.Identifier.GetLocation(),
                count.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
