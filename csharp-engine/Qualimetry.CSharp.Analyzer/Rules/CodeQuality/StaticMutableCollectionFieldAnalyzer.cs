using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StaticMutableCollectionFieldAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> MutableCollectionTypes = ImmutableHashSet.Create(
        "System.Collections.Generic.List`1",
        "System.Collections.Generic.Dictionary`2",
        "System.Collections.Generic.HashSet`1",
        "System.Collections.Generic.Queue`1",
        "System.Collections.Generic.Stack`1",
        "System.Collections.Generic.SortedList`2",
        "System.Collections.Generic.SortedDictionary`2",
        "System.Collections.Generic.SortedSet`1",
        "System.Collections.Generic.LinkedList`1",
        "System.Collections.ObjectModel.Collection`1",
        "System.Collections.ObjectModel.ObservableCollection`1");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0027);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (!field.Modifiers.Any(SyntaxKind.StaticKeyword)
            || field.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return;
        }

        if (!IsMutableCollectionType(context, field.Declaration.Type))
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not IFieldSymbol symbol)
            {
                continue;
            }

            if (!IsReachableOutsideDeclaringType(symbol.DeclaredAccessibility))
            {
                continue;
            }

            if (!ContainingTypeIsExternallyVisible(symbol.ContainingType))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0027,
                variable.Identifier.GetLocation()));
        }
    }

    private static bool IsReachableOutsideDeclaringType(Accessibility accessibility)
        => accessibility is Accessibility.Public
            or Accessibility.Internal
            or Accessibility.Protected
            or Accessibility.ProtectedOrInternal;

    private static bool ContainingTypeIsExternallyVisible(INamedTypeSymbol? containingType)
    {
        for (INamedTypeSymbol? current = containingType; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility is not (Accessibility.Public
                or Accessibility.Protected
                or Accessibility.ProtectedOrInternal))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsMutableCollectionType(SyntaxNodeAnalysisContext context, TypeSyntax type)
    {
        if (context.SemanticModel.GetTypeInfo(type, context.CancellationToken).Type is not INamedTypeSymbol named)
        {
            return false;
        }

        var definition = named.OriginalDefinition;
        var fullName = definition.ContainingNamespace?.ToDisplayString() + "." + definition.MetadataName;
        return MutableCollectionTypes.Contains(fullName);
    }
}
