using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExternalFieldAssignmentAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0001);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleAssignmentExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        if (assignment.Parent is InitializerExpressionSyntax)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(assignment.Left, context.CancellationToken).Symbol is not IFieldSymbol field)
        {
            return;
        }

        if (field.IsConst || field.IsImplicitlyDeclared)
        {
            return;
        }

        var declaringType = field.ContainingType;
        if (declaringType is null)
        {
            return;
        }

        var enclosingType = context.ContainingSymbol?.ContainingType;
        if (enclosingType is null)
        {
            return;
        }

        if (IsWithinHierarchy(enclosingType, declaringType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0001, assignment.Left.GetLocation()));
    }

    private static bool IsWithinHierarchy(INamedTypeSymbol enclosingType, INamedTypeSymbol declaringType)
    {
        for (INamedTypeSymbol? current = enclosingType; current is not null; current = current.ContainingType)
        {
            if (InheritsFromOrEquals(current, declaringType) || InheritsFromOrEquals(declaringType, current))
            {
                return true;
            }
        }

        return false;
    }

    private static bool InheritsFromOrEquals(INamedTypeSymbol type, INamedTypeSymbol candidate)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, candidate))
            {
                return true;
            }
        }

        return false;
    }
}
