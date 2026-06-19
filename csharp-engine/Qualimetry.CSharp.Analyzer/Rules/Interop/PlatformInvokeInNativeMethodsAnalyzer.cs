using System.Collections.Immutable;
using System.Linq;
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
        if (!HasDllImportAttribute(method))
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

    private static bool HasDllImportAttribute(MethodDeclarationSyntax method)
    {
        return method.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attribute => IsDllImport(attribute.Name));
    }

    private static bool IsDllImport(NameSyntax name)
    {
        var identifier = name switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            SimpleNameSyntax simple => simple.Identifier.ValueText,
            _ => null,
        };

        return identifier is "DllImport" or "DllImportAttribute";
    }
}
