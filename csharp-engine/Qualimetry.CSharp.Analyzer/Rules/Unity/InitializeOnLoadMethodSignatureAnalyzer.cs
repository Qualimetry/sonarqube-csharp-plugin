using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InitializeOnLoadMethodSignatureAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0097);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        var hasLoadAttribute = UnityAttributes.Has(method.AttributeLists, "InitializeOnLoadMethod")
            || UnityAttributes.Has(method.AttributeLists, "RuntimeInitializeOnLoadMethod");

        if (!hasLoadAttribute)
        {
            return;
        }

        var isStatic = method.Modifiers.Any(SyntaxKind.StaticKeyword);
        var isParameterless = method.ParameterList.Parameters.Count == 0;

        if (isStatic && isParameterless)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0097, method.Identifier.GetLocation()));
    }
}
