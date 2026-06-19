using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedPrivateMethodAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0167);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!method.Modifiers.Any(SyntaxKind.PrivateKeyword) || method.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        if (method.AttributeLists.Count > 0 || method.ExplicitInterfaceSpecifier != null)
        {
            return;
        }

        var name = method.Identifier.ValueText;
        if (name == "Main")
        {
            return;
        }

        if (!(method.Parent is TypeDeclarationSyntax typeDeclaration) || typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        foreach (var node in typeDeclaration.DescendantNodes())
        {
            if (node == method)
            {
                continue;
            }

            string? candidate = null;
            if (node is IdentifierNameSyntax identifier)
            {
                candidate = identifier.Identifier.ValueText;
            }
            else if (node is GenericNameSyntax generic)
            {
                candidate = generic.Identifier.ValueText;
            }

            if (candidate == name)
            {
                return;
            }
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0167, method.Identifier.GetLocation(), name));
    }
}
