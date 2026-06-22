using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Interop;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PlatformInvokeInNativeMethodsAnalyzer : DiagnosticAnalyzer
{
    private const string ContainerSuffix = "NativeMethods";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0116);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!PlatformInvokeAttribute.IsPresent(method, context.SemanticModel))
        {
            return;
        }

        if (method.Parent is not TypeDeclarationSyntax containingType)
        {
            return;
        }

        var name = containingType.Identifier.ValueText;
        if (name.EndsWith(ContainerSuffix, System.StringComparison.Ordinal))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0116, method.Identifier.GetLocation()));
    }
}
