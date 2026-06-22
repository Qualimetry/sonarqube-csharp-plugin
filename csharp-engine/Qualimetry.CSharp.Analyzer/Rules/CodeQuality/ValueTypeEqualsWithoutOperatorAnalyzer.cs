using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ValueTypeEqualsWithoutOperatorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0122);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (StructDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } typeSymbol)
        {
            return;
        }

        if (!OverridesEquals(typeSymbol))
        {
            return;
        }

        if (typeSymbol.GetMembers("op_Equality").Any())
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0122, declaration.Identifier.GetLocation()));
    }

    private static bool OverridesEquals(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers("Equals").OfType<IMethodSymbol>().Any(m =>
            m.IsOverride
            && m.Parameters.Length == 1
            && m.Parameters[0].Type.SpecialType == SpecialType.System_Object);
    }
}
