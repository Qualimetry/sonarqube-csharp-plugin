using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExcessiveMethodOverloadsAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxOverloads = 6;
    private const string Option = "qualimetry.qa_metrics_excessive_method_overloads.maxoverloads";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0021);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.InterfaceDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;
        var threshold = OptionReader.ReadInt(context, declaration.SyntaxTree, Option, DefaultMaxOverloads);

        var counts = new Dictionary<string, int>(System.StringComparer.Ordinal);
        foreach (var member in declaration.Members)
        {
            if (member is MethodDeclarationSyntax method)
            {
                var name = method.Identifier.ValueText;
                counts.TryGetValue(name, out var current);
                counts[name] = current + 1;
            }
        }

        foreach (var pair in counts)
        {
            if (pair.Value > threshold)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0021,
                    declaration.Identifier.GetLocation(),
                    pair.Key,
                    pair.Value.ToString(CultureInfo.InvariantCulture),
                    threshold.ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}
