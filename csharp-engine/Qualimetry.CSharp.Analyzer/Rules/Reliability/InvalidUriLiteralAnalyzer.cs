using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidUriLiteralAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0199);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ObjectCreationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;

        if (SimpleTypeName(creation.Type) != "Uri")
        {
            return;
        }

        var arguments = creation.ArgumentList?.Arguments;
        if (arguments is null || arguments.Value.Count == 0)
        {
            return;
        }

        if (arguments.Value[0].Expression is not LiteralExpressionSyntax literal
            || !literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return;
        }

        if (arguments.Value.Count >= 2 && !SpecifiesAbsoluteKind(arguments.Value[1].Expression))
        {
            return;
        }

        if (!Uri.TryCreate(literal.Token.ValueText, UriKind.Absolute, out _))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0199, literal.GetLocation()));
        }
    }

    private static bool SpecifiesAbsoluteKind(ExpressionSyntax expression)
    {
        return expression is MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Absolute" };
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
