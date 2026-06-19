using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThrowInStaticConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0077);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ConstructorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var constructor = (ConstructorDeclarationSyntax)context.Node;

        if (!constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        SyntaxNode? body = constructor.Body ?? (SyntaxNode?)constructor.ExpressionBody;
        if (body is null)
        {
            return;
        }

        foreach (var node in body.DescendantNodes())
        {
            if (node is not ThrowStatementSyntax && node is not ThrowExpressionSyntax)
            {
                continue;
            }

            if (IsInsideNestedScope(node, body))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0077, node.GetLocation()));
        }
    }

    private static bool IsInsideNestedScope(SyntaxNode node, SyntaxNode body)
    {
        for (var current = node.Parent; current is not null && current != body; current = current.Parent)
        {
            if (current is LambdaExpressionSyntax or AnonymousMethodExpressionSyntax or LocalFunctionStatementSyntax)
            {
                return true;
            }
        }

        return false;
    }
}
