using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class VirtualPropertyAccessInConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0184);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ConstructorDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var constructor = (ConstructorDeclarationSyntax)context.Node;

        if (constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(constructor, context.CancellationToken)?.ContainingType is not { } containingType
            || containingType.IsSealed)
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
            var candidate = CandidateAccess(node);
            if (candidate is null)
            {
                continue;
            }

            if (context.SemanticModel.GetSymbolInfo(candidate, context.CancellationToken).Symbol is not IPropertySymbol property)
            {
                continue;
            }

            if (!(property.IsVirtual || property.IsAbstract || property.IsOverride))
            {
                continue;
            }

            if (!TypeChainContains(containingType, property.ContainingType))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0184, candidate.GetLocation()));
            return;
        }
    }

    private static ExpressionSyntax? CandidateAccess(SyntaxNode node)
    {
        switch (node)
        {
            case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is ThisExpressionSyntax:
                return memberAccess.Name;
            case IdentifierNameSyntax identifier when IsCurrentInstanceIdentifier(identifier):
                return identifier;
            default:
                return null;
        }
    }

    private static bool IsCurrentInstanceIdentifier(IdentifierNameSyntax identifier)
    {
        if (identifier.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == identifier)
        {
            return false;
        }

        if (identifier.Parent is QualifiedNameSyntax)
        {
            return false;
        }

        return true;
    }

    private static bool TypeChainContains(INamedTypeSymbol type, INamedTypeSymbol? target)
    {
        if (target is null)
        {
            return false;
        }

        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, target))
            {
                return true;
            }
        }

        return false;
    }
}
