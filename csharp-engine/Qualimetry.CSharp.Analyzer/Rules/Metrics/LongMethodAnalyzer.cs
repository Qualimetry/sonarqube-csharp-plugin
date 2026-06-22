using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LongMethodAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxStatements = 40;
    private const string Option = "qualimetry.qa_metrics_long_method.maxstatements";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0019);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.Body == null)
        {
            return;
        }

        var threshold = OptionReader.ReadInt(context, method.SyntaxTree, Option, DefaultMaxStatements);

        var count = method.Body
            .DescendantNodes(descendIntoChildren: n => n is not LocalFunctionStatementSyntax)
            .Count(n => n is StatementSyntax && n is not BlockSyntax);

        if (count > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0019,
                method.Identifier.GetLocation(),
                method.Identifier.ValueText,
                count.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
