using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OverrideDoesNotCallBaseDisposeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0123);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!method.Modifiers.Any(SyntaxKind.OverrideKeyword))
        {
            return;
        }

        if (method.Identifier.ValueText != "Dispose"
            || method.ParameterList.Parameters.Count != 1)
        {
            return;
        }

        var parameter = method.ParameterList.Parameters[0];
        if (parameter.Type is not PredefinedTypeSyntax predefined
            || !predefined.Keyword.IsKind(SyntaxKind.BoolKeyword))
        {
            return;
        }

        SyntaxNode? body = (SyntaxNode?)method.Body ?? method.ExpressionBody;
        if (body is null)
        {
            return;
        }

        if (CallsBaseDispose(body))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0123, method.Identifier.GetLocation()));
    }

    private static bool CallsBaseDispose(SyntaxNode body)
    {
        foreach (var invocation in body.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess
                && memberAccess.Expression is BaseExpressionSyntax
                && memberAccess.Name.Identifier.ValueText == "Dispose")
            {
                return true;
            }
        }

        return false;
    }
}
