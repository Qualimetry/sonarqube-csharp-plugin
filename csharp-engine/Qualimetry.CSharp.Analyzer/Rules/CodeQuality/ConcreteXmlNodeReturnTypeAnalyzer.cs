using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ConcreteXmlNodeReturnTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0108);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is not { } symbol)
        {
            return;
        }

        if (symbol.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal))
        {
            return;
        }

        if (!ContainingTypeIsExternallyVisible(symbol.ContainingType))
        {
            return;
        }

        if (!IsConcreteXmlNode(symbol.ReturnType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0108, method.ReturnType.GetLocation()));
    }

    private static bool ContainingTypeIsExternallyVisible(INamedTypeSymbol? containingType)
    {
        for (INamedTypeSymbol? current = containingType; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is not (Accessibility.Public
                or Accessibility.Protected
                or Accessibility.ProtectedOrInternal))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsConcreteXmlNode(ITypeSymbol type)
    {
        if (type.ContainingNamespace?.ToDisplayString() != "System.Xml")
        {
            return false;
        }

        return type.Name is "XmlNode" or "XmlElement" or "XmlDocument" or "XmlAttribute" or "XmlDocumentFragment";
    }
}
