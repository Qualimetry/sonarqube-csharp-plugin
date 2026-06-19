using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TypePoorCohesionAnalyzer : DiagnosticAnalyzer
{
    private const int MinFields = 4;
    private const int MinMethods = 4;
    private const double MinDisjointRatio = 0.75;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Descriptors.QCS0210);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(declaration);
        if (typeSymbol == null || ShouldSkip(typeSymbol))
        {
            return;
        }

        var fields = new List<IFieldSymbol>();
        var methods = new List<IMethodSymbol>();

        foreach (ISymbol member in typeSymbol.GetMembers())
        {
            if (member is IFieldSymbol field
                && !field.IsStatic
                && !field.IsConst
                && field.AssociatedSymbol == null)
            {
                fields.Add(field);
            }
            else if (member is IMethodSymbol method
                && !method.IsStatic
                && method.MethodKind == MethodKind.Ordinary)
            {
                methods.Add(method);
            }
        }

        if (fields.Count < MinFields || methods.Count < MinMethods)
        {
            return;
        }

        var fieldSets = methods
            .Select(method => new HashSet<IFieldSymbol>(
                GetAccessedInstanceFields(method, fields),
                SymbolEqualityComparer.Default))
            .ToList();

        int pairs = 0;
        int disjointPairs = 0;
        for (int i = 0; i < fieldSets.Count; i++)
        {
            for (int j = i + 1; j < fieldSets.Count; j++)
            {
                pairs++;
                if (!fieldSets[i].Overlaps(fieldSets[j]))
                {
                    disjointPairs++;
                }
            }
        }

        if (pairs == 0 || ((double)disjointPairs / pairs) < MinDisjointRatio)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0210,
            declaration.Identifier.GetLocation(),
            typeSymbol.Name));
    }

    private static bool ShouldSkip(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind == TypeKind.Struct)
        {
            return true;
        }

        if (typeSymbol.IsRecord)
        {
            return true;
        }

        string name = typeSymbol.Name;
        return name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("DTO", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("Data", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("EventArgs", StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<IFieldSymbol> GetAccessedInstanceFields(IMethodSymbol method, IReadOnlyList<IFieldSymbol> fields)
    {
        foreach (SyntaxReference reference in method.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax() is not MethodDeclarationSyntax syntax)
            {
                continue;
            }

            foreach (SyntaxNode node in syntax.DescendantNodes())
            {
                if (node is not IdentifierNameSyntax identifier)
                {
                    continue;
                }

                IFieldSymbol? field = fields.FirstOrDefault(
                    candidate => string.Equals(candidate.Name, identifier.Identifier.ValueText, StringComparison.Ordinal));
                if (field != null)
                {
                    yield return field;
                }
            }
        }
    }
}
