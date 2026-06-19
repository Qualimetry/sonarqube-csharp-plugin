using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExcessiveFieldCountAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxFields = 15;
    private const string Option = "qualimetry.qa_metrics_excessive_field_count.maxfields";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0031);

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
        var threshold = OptionReader.ReadInt(context, declaration.SyntaxTree, Option, DefaultMaxFields);

        var count = 0;
        foreach (var member in declaration.Members)
        {
            if (member is FieldDeclarationSyntax field && !field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                count += field.Declaration.Variables.Count;
            }
        }

        if (count > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0031,
                declaration.Identifier.GetLocation(),
                count.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
