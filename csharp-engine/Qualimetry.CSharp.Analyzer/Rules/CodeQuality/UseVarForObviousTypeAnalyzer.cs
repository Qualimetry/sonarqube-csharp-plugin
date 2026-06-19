using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseVarForObviousTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0192);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.LocalDeclarationStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var statement = (LocalDeclarationStatementSyntax)context.Node;

        if (statement.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return;
        }

        var declaration = statement.Declaration;
        if (declaration.Type.IsVar || declaration.Variables.Count != 1)
        {
            return;
        }

        if (declaration.Variables[0].Initializer?.Value is not ObjectCreationExpressionSyntax creation)
        {
            return;
        }

        var declaredType = context.SemanticModel.GetTypeInfo(declaration.Type, context.CancellationToken).Type;
        var createdType = context.SemanticModel.GetTypeInfo(creation, context.CancellationToken).Type;

        if (declaredType is null
            || createdType is null
            || declaredType.TypeKind == TypeKind.Error
            || !SymbolEqualityComparer.Default.Equals(declaredType, createdType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0192, declaration.Type.GetLocation()));
    }
}
