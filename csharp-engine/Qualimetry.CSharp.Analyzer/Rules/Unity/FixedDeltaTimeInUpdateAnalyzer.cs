using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FixedDeltaTimeInUpdateAnalyzer : DiagnosticAnalyzer
{
    private const string TimeType = "Time";
    private const string FixedDeltaMember = "fixedDeltaTime";
    private const string BehaviourBaseName = "MonoBehaviour";

    private static readonly ImmutableHashSet<string> FrameMethods = ImmutableHashSet.Create(
        "Update",
        "LateUpdate");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0035);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!FrameMethods.Contains(method.Identifier.ValueText))
        {
            return;
        }

        SyntaxNode? body = (SyntaxNode?)method.Body ?? method.ExpressionBody;
        if (body is null)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method) is not IMethodSymbol methodSymbol
            || !UnityTypeNames.DerivesFrom(methodSymbol.ContainingType, BehaviourBaseName))
        {
            return;
        }

        foreach (var node in body.DescendantNodes())
        {
            if (node is MemberAccessExpressionSyntax access
                && access.Name.Identifier.ValueText == FixedDeltaMember
                && access.Expression is IdentifierNameSyntax owner
                && owner.Identifier.ValueText == TimeType
                && UnityTypeNames.IsUnityType(context.SemanticModel.GetSymbolInfo(access).Symbol?.ContainingType, TimeType))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0035, access.GetLocation()));
            }
        }
    }
}
