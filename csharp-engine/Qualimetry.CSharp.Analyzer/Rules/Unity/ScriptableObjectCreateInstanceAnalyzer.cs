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
        var derivedTypes = new ConcurrentDictionary<INamedTypeSymbol, byte>(SymbolEqualityComparer.Default);
        var constructions = new ConcurrentBag<(Location Location, INamedTypeSymbol Type)>();

        context.RegisterSymbolAction(
            symbolContext =>
            {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                if (UnityTypeNames.DerivesFrom(type.BaseType, ScriptableObjectBaseName))
                {
                    derivedTypes.TryAdd(type, 0);
                }
            },
            SymbolKind.NamedType);

        context.RegisterSyntaxNodeAction(
            nodeContext =>
            {
                var creation = (ObjectCreationExpressionSyntax)nodeContext.Node;
                if (nodeContext.SemanticModel.GetSymbolInfo(creation).Symbol is IMethodSymbol constructor
                    && constructor.ContainingType is { } createdType)
                {
                    constructions.Add((creation.Type.GetLocation(), createdType));
                }
            },
            SyntaxKind.ObjectCreationExpression);

        context.RegisterCompilationEndAction(
            endContext =>
            {
                foreach (var (location, type) in constructions)
                {
                    if (derivedTypes.ContainsKey(type))
                    {
                        endContext.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0172, location));
                    }
                }
            });
    }
}
