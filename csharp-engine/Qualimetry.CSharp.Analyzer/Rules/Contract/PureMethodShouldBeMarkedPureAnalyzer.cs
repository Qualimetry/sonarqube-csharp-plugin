using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Contract;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PureMethodShouldBeMarkedPureAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0136);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (method.ExpressionBody is null)
        {
            return;
        }

        if (method.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            return;
        }

        if (method.Modifiers.Any(SyntaxKind.OverrideKeyword) || method.ExplicitInterfaceSpecifier is not null)
        {
            return;
        }

        if (method.ReturnType is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword))
        {
            return;
        }

        if (HasPureAttribute(method))
        {
            return;
        }

        if (!IsSideEffectFree(method.ExpressionBody.Expression))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0136,
            method.Identifier.GetLocation(),
            method.Identifier.ValueText));
    }

    private static bool HasPureAttribute(MethodDeclarationSyntax method)
    {
        return method.AttributeLists
            .SelectMany(list => list.Attributes)
            .Select(attribute => attribute.Name)
            .Any(IsPureName);
    }

    private static bool IsPureName(NameSyntax name)
    {
        var identifier = name switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            SimpleNameSyntax simple => simple.Identifier.ValueText,
            _ => null,
        };

        return identifier is "Pure" or "PureAttribute";
    }

    private static bool IsSideEffectFree(ExpressionSyntax expression)
    {
        foreach (var node in expression.DescendantNodesAndSelf())
        {
            switch (node)
            {
                case InvocationExpressionSyntax:
                case ObjectCreationExpressionSyntax:
                case ImplicitObjectCreationExpressionSyntax:
                case AwaitExpressionSyntax:
                case AssignmentExpressionSyntax:
                case ThrowExpressionSyntax:
                case ThrowStatementSyntax:
                case StackAllocArrayCreationExpressionSyntax:
                case ImplicitStackAllocArrayCreationExpressionSyntax:
                    return false;
                case PostfixUnaryExpressionSyntax postfix when IsIncrementOrDecrement(postfix.OperatorToken):
                    return false;
                case PrefixUnaryExpressionSyntax prefix when IsIncrementOrDecrement(prefix.OperatorToken):
                    return false;
            }
        }

        return true;
    }

    private static bool IsIncrementOrDecrement(SyntaxToken token)
    {
        return token.IsKind(SyntaxKind.PlusPlusToken) || token.IsKind(SyntaxKind.MinusMinusToken);
    }
}
