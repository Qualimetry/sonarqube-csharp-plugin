using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ScriptableObjectCreateInstanceAnalyzer : DiagnosticAnalyzer
{
    private const string ScriptableObjectBaseName = "ScriptableObject";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0172);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var derivedTypeNames = new ConcurrentDictionary<string, byte>();
        var constructions = new ConcurrentBag<(Location Location, string TypeName)>();

        context.RegisterSyntaxNodeAction(
            nodeContext =>
            {
                var declaration = (ClassDeclarationSyntax)nodeContext.Node;
                if (DerivesFromScriptableObject(declaration))
                {
                    derivedTypeNames.TryAdd(declaration.Identifier.ValueText, 0);
                }
            },
            SyntaxKind.ClassDeclaration);

        context.RegisterSyntaxNodeAction(
            nodeContext =>
            {
                var creation = (ObjectCreationExpressionSyntax)nodeContext.Node;
                var name = UnityTypeNames.SimpleName(creation.Type);
                if (name is not null)
                {
                    constructions.Add((creation.Type.GetLocation(), name));
                }
            },
            SyntaxKind.ObjectCreationExpression);

        context.RegisterCompilationEndAction(
            endContext =>
            {
                foreach (var (location, typeName) in constructions)
                {
                    if (derivedTypeNames.ContainsKey(typeName))
                    {
                        endContext.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0172, location));
                    }
                }
            });
    }

    private static bool DerivesFromScriptableObject(ClassDeclarationSyntax declaration)
    {
        if (declaration.BaseList is null)
        {
            return false;
        }

        foreach (var baseType in declaration.BaseList.Types)
        {
            if (UnityTypeNames.SimpleName(baseType.Type) == ScriptableObjectBaseName)
            {
                return true;
            }
        }

        return false;
    }
}
