using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MultipleTypesPerFileAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0010);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxTreeAction(Analyze);
    }

    private static void Analyze(SyntaxTreeAnalysisContext context)
    {
        var root = context.Tree.GetRoot(context.CancellationToken);
        var seenFirst = false;

        foreach (var node in root.DescendantNodes(descendIntoChildren: ShouldDescend))
        {
            if (node is not BaseTypeDeclarationSyntax && node is not DelegateDeclarationSyntax)
            {
                continue;
            }

            if (!IsTopLevel(node))
            {
                continue;
            }

            if (!seenFirst)
            {
                seenFirst = true;
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0010, GetIdentifier(node).GetLocation()));
        }
    }

    private static bool ShouldDescend(SyntaxNode node)
    {
        return node is CompilationUnitSyntax || node is BaseNamespaceDeclarationSyntax;
    }

    private static bool IsTopLevel(SyntaxNode node)
    {
        return node.Parent is CompilationUnitSyntax || node.Parent is BaseNamespaceDeclarationSyntax;
    }

    private static SyntaxToken GetIdentifier(SyntaxNode node)
    {
        return node switch
        {
            BaseTypeDeclarationSyntax type => type.Identifier,
            DelegateDeclarationSyntax del => del.Identifier,
            _ => node.GetFirstToken(),
        };
    }
}
