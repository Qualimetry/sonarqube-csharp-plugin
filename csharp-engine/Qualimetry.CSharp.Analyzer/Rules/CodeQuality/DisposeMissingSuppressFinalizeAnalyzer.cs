using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DisposeMissingSuppressFinalizeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0060);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (!declaration.Members.OfType<DestructorDeclarationSyntax>().Any())
        {
            return;
        }

        var dispose = declaration.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(m =>
                m.Identifier.ValueText == "Dispose"
                && m.ParameterList.Parameters.Count == 0
                && !m.Modifiers.Any(SyntaxKind.StaticKeyword));

        if (dispose is null)
        {
            return;
        }

        if (CallsSuppressFinalize(dispose))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0060,
            dispose.Identifier.GetLocation()));
    }

    private static bool CallsSuppressFinalize(MethodDeclarationSyntax dispose)
    {
        SyntaxNode? body = dispose.Body ?? (SyntaxNode?)dispose.ExpressionBody;
        if (body is null)
        {
            return false;
        }

        foreach (var invocation in body.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess
                && memberAccess.Name.Identifier.ValueText == "SuppressFinalize")
            {
                return true;
            }
        }

        return false;
    }
}
