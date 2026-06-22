using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Interop;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NativeMethodsContainerModifiersAnalyzer : DiagnosticAnalyzer
{
    private const string ContainerSuffix = "NativeMethods";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0117);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        var name = declaration.Identifier.ValueText;
        if (!name.EndsWith(ContainerSuffix, System.StringComparison.Ordinal))
        {
            return;
        }

        if (!ContainsPlatformInvoke(declaration, context.SemanticModel))
        {
            return;
        }

        var isPublic = declaration.Modifiers.Any(SyntaxKind.PublicKeyword);
        var isStatic = declaration.Modifiers.Any(SyntaxKind.StaticKeyword);
        if (!isPublic && isStatic)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0117, declaration.Identifier.GetLocation(), name));
    }

    private static bool ContainsPlatformInvoke(ClassDeclarationSyntax declaration, SemanticModel semanticModel)
    {
        return declaration.Members
            .OfType<MethodDeclarationSyntax>()
            .Any(method => PlatformInvokeAttribute.IsPresent(method, semanticModel));
    }
}
