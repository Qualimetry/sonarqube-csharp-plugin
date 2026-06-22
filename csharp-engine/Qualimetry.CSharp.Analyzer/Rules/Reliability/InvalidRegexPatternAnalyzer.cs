using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidRegexPatternAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> StaticPatternMethods = ImmutableHashSet.Create(
        "IsMatch",
        "Match",
        "Matches",
        "Replace",
        "Split");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0196);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeCreation(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;

        if (SimpleTypeName(creation.Type) != "Regex")
        {
            return;
        }

        var createdType = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;
        if (createdType?.ToDisplayString() != "System.Text.RegularExpressions.Regex")
        {
            return;
        }

        var arguments = creation.ArgumentList?.Arguments;
        if (arguments is null || arguments.Value.Count == 0)
        {
            return;
        }

        ValidatePattern(context, arguments.Value[0].Expression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess
            || !StaticPatternMethods.Contains(memberAccess.Name.Identifier.ValueText)
            || memberAccess.Expression is not IdentifierNameSyntax { Identifier.ValueText: "Regex" })
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method
            || method.ContainingType?.ToDisplayString() != "System.Text.RegularExpressions.Regex")
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count < 2)
        {
            return;
        }

        ValidatePattern(context, arguments[1].Expression);
    }

    private static void ValidatePattern(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        if (expression is not LiteralExpressionSyntax literal
            || !literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return;
        }

        var pattern = literal.Token.ValueText;
        try
        {
            _ = new Regex(pattern);
        }
        catch (ArgumentException)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0196, literal.GetLocation()));
        }
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
