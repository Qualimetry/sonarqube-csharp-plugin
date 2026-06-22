using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class VirtualCallInConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0056);

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

        if (context.SemanticModel.GetDeclaredSymbol(constructor)?.ContainingType is not { } containingType)
        {
            return;
        }

        if (containingType.IsSealed)
        {
            return;
        }

        SyntaxNode? body = constructor.Body ?? (SyntaxNode?)constructor.ExpressionBody;
        if (body is null)
        {
            return;
        }

        foreach (var invocation in body.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (!IsCurrentInstanceReceiver(invocation.Expression))
            {
                continue;
            }

            if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol method)
            {
                continue;
            }

            if (method.IsStatic || !(method.IsVirtual || method.IsAbstract || method.IsOverride))
            {
                continue;
            }

            if (!ContainingTypeChainContains(containingType, method.ContainingType))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0056,
                invocation.GetLocation(),
                method.Name));
            return;
        }
    }

    private static bool IsCurrentInstanceReceiver(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax => true,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Expression is ThisExpressionSyntax,
            _ => false,
        };
    }

    private static bool ContainingTypeChainContains(INamedTypeSymbol type, INamedTypeSymbol? target)
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
