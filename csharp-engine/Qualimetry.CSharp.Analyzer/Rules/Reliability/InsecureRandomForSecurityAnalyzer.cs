using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InsecureRandomForSecurityAnalyzer : DiagnosticAnalyzer
{
    private static readonly string[] SecurityKeywords =
    {
        "token", "key", "secret", "password", "passwd", "pwd", "salt", "nonce",
        "iv", "otp", "cipher", "crypto", "session", "auth", "credential", "apikey",
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0083);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;

        if (SimpleTypeName(creation.Type) != "Random")
        {
            return;
        }

        var contextName = ResolveBindingName(creation);
        if (contextName is null || !SuggestsSecurityUse(contextName))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0083, creation.GetLocation()));
    }

    private static string? ResolveBindingName(ObjectCreationExpressionSyntax creation)
    {
        SyntaxNode? node = creation.Parent;
        while (node is not null)
        {
            switch (node)
            {
                case EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax declarator }:
                    return declarator.Identifier.ValueText;
                case AssignmentExpressionSyntax assignment when assignment.Right == creation || Contains(assignment.Right, creation):
                    return LeftName(assignment.Left);
                case PropertyDeclarationSyntax property:
                    return property.Identifier.ValueText;
                case FieldDeclarationSyntax field when field.Declaration.Variables.Count > 0:
                    return field.Declaration.Variables[0].Identifier.ValueText;
                case MethodDeclarationSyntax:
                case StatementSyntax:
                    return null;
            }

            node = node.Parent;
        }

        return null;
    }

    private static bool Contains(SyntaxNode root, SyntaxNode target) => root == target || root.Contains(target);

    private static string? LeftName(ExpressionSyntax left)
    {
        return left switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            _ => null,
        };
    }

    private static bool SuggestsSecurityUse(string name)
    {
        foreach (var keyword in SecurityKeywords)
        {
            if (name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private static string? SimpleTypeName(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            _ => null,
        };
    }
}
