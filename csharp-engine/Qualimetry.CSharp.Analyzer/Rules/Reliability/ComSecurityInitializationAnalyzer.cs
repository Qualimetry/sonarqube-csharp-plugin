using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ComSecurityInitializationAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> SecurityFunctions = ImmutableHashSet.Create(
        "CoInitializeSecurity",
        "CoSetProxyBlanket");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0079);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        var nameNode = invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name,
            IdentifierNameSyntax identifier => identifier,
            _ => null,
        };

        if (nameNode is not IdentifierNameSyntax plainName)
        {
            return;
        }

        if (!SecurityFunctions.Contains(plainName.Identifier.ValueText))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0079, plainName.GetLocation()));
    }
}
