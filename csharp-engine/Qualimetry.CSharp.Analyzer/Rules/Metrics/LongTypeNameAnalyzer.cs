using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LongTypeNameAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxNameLength = 40;
    private const string Option = "qualimetry.qa_metrics_long_type_name.maxnamelength";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0030);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.EnumDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var identifier = context.Node switch
        {
            BaseTypeDeclarationSyntax baseType => baseType.Identifier,
            _ => default,
        };

        if (identifier == default)
        {
            return;
        }

        var threshold = OptionReader.ReadInt(context, context.Node.SyntaxTree, Option, DefaultMaxNameLength);
        var name = identifier.ValueText;
        if (name.Length > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0030,
                identifier.GetLocation(),
                name,
                name.Length.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
