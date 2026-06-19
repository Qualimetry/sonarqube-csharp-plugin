using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EmptyUnityMessageAnalyzer : DiagnosticAnalyzer
{
    private const string BehaviourBaseName = "MonoBehaviour";

    private static readonly ImmutableHashSet<string> LifecycleMethods = ImmutableHashSet.Create(
        "Awake",
        "OnEnable",
        "Start",
        "FixedUpdate",
        "Update",
        "LateUpdate",
        "OnDisable",
        "OnDestroy",
        "OnGUI",
        "OnApplicationQuit");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0012);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!LifecycleMethods.Contains(method.Identifier.ValueText))
        {
            return;
        }

        if (method.Body is not { Statements.Count: 0 })
        {
            return;
        }

        if (method.Parent is not ClassDeclarationSyntax declaringClass || !DerivesFromBehaviour(declaringClass))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0012, method.Identifier.GetLocation()));
    }

    private static bool DerivesFromBehaviour(ClassDeclarationSyntax declaration)
    {
        if (declaration.BaseList is null)
        {
            return false;
        }

        foreach (var baseType in declaration.BaseList.Types)
        {
            if (UnityTypeNames.SimpleName(baseType.Type) == BehaviourBaseName)
            {
                return true;
            }
        }

        return false;
    }
}
