using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExcessiveParameterCountAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxParameters = 7;
    private const string Option = "qualimetry.qa_metrics_excessive_parameter_count.maxparameters";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0022);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        int parameterCount;
        SyntaxToken identifier;
        string label;

        switch (context.Node)
        {
            case MethodDeclarationSyntax method:
                parameterCount = method.ParameterList.Parameters.Count;
                identifier = method.Identifier;
                label = method.Identifier.ValueText;
                break;
            case ConstructorDeclarationSyntax ctor:
                parameterCount = ctor.ParameterList.Parameters.Count;
                identifier = ctor.Identifier;
                label = ctor.Identifier.ValueText + " constructor";
                break;
            default:
                return;
        }

        var threshold = OptionReader.ReadInt(context, context.Node.SyntaxTree, Option, DefaultMaxParameters);
        if (parameterCount > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0022,
                identifier.GetLocation(),
                label,
                parameterCount.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
