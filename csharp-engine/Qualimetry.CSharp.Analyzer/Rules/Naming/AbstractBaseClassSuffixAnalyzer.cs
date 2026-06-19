using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AbstractBaseClassSuffixAnalyzer : DiagnosticAnalyzer
{
    private const string DefaultSuffix = "Base";
    private const string SuffixOption = "qualimetry.qa_naming_abstract_base_class_suffix.suffix";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0003);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;
        if (!declaration.Modifiers.Any(SyntaxKind.AbstractKeyword))
        {
            return;
        }

        var suffix = OptionReader.ReadString(context, declaration.SyntaxTree, SuffixOption, DefaultSuffix);

        var name = declaration.Identifier.ValueText;
        if (name.EndsWith(suffix, System.StringComparison.Ordinal))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0003, declaration.Identifier.GetLocation(), name));
    }
}
