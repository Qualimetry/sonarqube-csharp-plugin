using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UndisposedDisposableLocalAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0114);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.LocalDeclarationStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (LocalDeclarationStatementSyntax)context.Node;

        if (!declaration.UsingKeyword.IsKind(SyntaxKind.None))
        {
            return;
        }

        var body = EnclosingBody(declaration);
        if (body is null)
        {
            return;
        }

        foreach (var variable in declaration.Declaration.Variables)
        {
            if (variable.Initializer?.Value is not ObjectCreationExpressionSyntax creation)
            {
                continue;
            }

            var type = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;
            if (!IsDisposable(type))
            {
                continue;
            }

            if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not ILocalSymbol local)
            {
                continue;
            }

            if (IsOwnershipHandled(context, body, local))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0114, creation.GetLocation()));
        }
    }

    private static SyntaxNode? EnclosingBody(SyntaxNode node)
    {
        for (var current = node.Parent; current is not null; current = current.Parent)
        {
            switch (current)
            {
                case BaseMethodDeclarationSyntax method:
                    return (SyntaxNode?)method.Body ?? method.ExpressionBody;
                case AccessorDeclarationSyntax accessor:
                    return (SyntaxNode?)accessor.Body ?? accessor.ExpressionBody;
                case LocalFunctionStatementSyntax localFunction:
                    return (SyntaxNode?)localFunction.Body ?? localFunction.ExpressionBody;
                case AnonymousFunctionExpressionSyntax anonymous:
                    return anonymous.Body;
            }
        }

        return null;
    }

    private static bool IsOwnershipHandled(SyntaxNodeAnalysisContext context, SyntaxNode body, ILocalSymbol local)
    {
        foreach (var identifier in body.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (identifier.Identifier.ValueText != local.Name)
            {
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(
                    context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol, local))
            {
                continue;
            }

            if (IsHandledUsage(identifier))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsHandledUsage(IdentifierNameSyntax identifier)
    {
        foreach (var ancestor in identifier.Ancestors())
        {
            switch (ancestor)
            {
                case ReturnStatementSyntax:
                case ArrowExpressionClauseSyntax:
                case ArgumentSyntax:
                case UsingStatementSyntax:
                case InitializerExpressionSyntax:
                    return true;
                case AssignmentExpressionSyntax assignment:
                    if (assignment.Left == identifier)
                    {
                        return true;
                    }

                    if (assignment.Right.DescendantNodesAndSelf().Contains(identifier))
                    {
                        return true;
                    }

                    break;
                case StatementSyntax:
                    break;
            }
        }

        if (identifier.Parent is MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Dispose" or "DisposeAsync" } memberAccess
            && memberAccess.Expression == identifier)
        {
            return true;
        }

        return false;
    }

    private static bool IsDisposable(ITypeSymbol? type)
    {
        if (type is null)
        {
            return false;
        }

        foreach (var @interface in type.AllInterfaces)
        {
            if ((@interface.Name == "IDisposable" || @interface.Name == "IAsyncDisposable")
                && @interface.ContainingNamespace?.ToDisplayString() == "System")
            {
                return true;
            }
        }

        return false;
    }
}
