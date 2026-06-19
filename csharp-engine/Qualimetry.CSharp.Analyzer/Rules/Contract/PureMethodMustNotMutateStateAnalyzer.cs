using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Contract;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PureMethodMustNotMutateStateAnalyzer : DiagnosticAnalyzer
{
    private const string PureAttributeMetadataName = "System.Diagnostics.Contracts.PureAttribute";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0112);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        SyntaxNode? body = method.Body ?? (SyntaxNode?)method.ExpressionBody;
        if (body is null)
        {
            return;
        }

        var model = context.SemanticModel;
        if (!IsMarkedPure(method, model))
        {
            return;
        }

        if (!MutatesObservableState(body, model))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0112,
            method.Identifier.GetLocation(),
            method.Identifier.ValueText));
    }

    private static bool IsMarkedPure(MethodDeclarationSyntax method, SemanticModel model)
    {
        foreach (var attribute in method.AttributeLists.SelectMany(list => list.Attributes))
        {
            if (model.GetSymbolInfo(attribute).Symbol is IMethodSymbol constructor
                && constructor.ContainingType?.ToDisplayString() == PureAttributeMetadataName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool MutatesObservableState(SyntaxNode body, SemanticModel model)
    {
        foreach (var node in body.DescendantNodes())
        {
            switch (node)
            {
                case AssignmentExpressionSyntax assignment when WritesMember(assignment.Left, model):
                    return true;
                case PostfixUnaryExpressionSyntax postfix when IsIncrementOrDecrement(postfix.OperatorToken) && WritesMember(postfix.Operand, model):
                    return true;
                case PrefixUnaryExpressionSyntax prefix when IsIncrementOrDecrement(prefix.OperatorToken) && WritesMember(prefix.Operand, model):
                    return true;
            }
        }

        return false;
    }

    private static bool IsIncrementOrDecrement(SyntaxToken token)
    {
        return token.IsKind(SyntaxKind.PlusPlusToken) || token.IsKind(SyntaxKind.MinusMinusToken);
    }

    private static bool WritesMember(ExpressionSyntax target, SemanticModel model)
    {
        var expression = Unwrap(target);

        switch (expression)
        {
            case IdentifierNameSyntax identifier:
                return model.GetSymbolInfo(identifier).Symbol is IFieldSymbol or IPropertySymbol;

            case MemberAccessExpressionSyntax memberAccess:
                if (model.GetSymbolInfo(memberAccess).Symbol is not (IFieldSymbol or IPropertySymbol))
                {
                    return false;
                }

                if (memberAccess.Expression is ThisExpressionSyntax)
                {
                    return true;
                }

                return model.GetSymbolInfo(memberAccess.Expression).Symbol is INamedTypeSymbol;

            default:
                return false;
        }
    }

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            expression = parenthesized.Expression;
        }

        return expression;
    }
}
