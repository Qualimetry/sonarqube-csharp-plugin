using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MutableCollectionPropertySetterAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> CollectionTypeNames = ImmutableHashSet.Create(
        "List",
        "Dictionary",
        "HashSet",
        "SortedSet",
        "SortedList",
        "SortedDictionary",
        "Collection",
        "ObservableCollection",
        "ICollection",
        "IList",
        "IDictionary",
        "ISet",
        "ArrayList",
        "Hashtable",
        "Queue",
        "Stack",
        "LinkedList");

    private static readonly ImmutableHashSet<string> CollectionNamespaces = ImmutableHashSet.Create(
        "System.Collections",
        "System.Collections.Generic",
        "System.Collections.ObjectModel");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0051);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.PropertyDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;

        if (property.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        if (!property.Modifiers.Any(SyntaxKind.PublicKeyword)
            && !property.Modifiers.Any(SyntaxKind.ProtectedKeyword))
        {
            return;
        }

        var propertyType = context.SemanticModel.GetTypeInfo(property.Type, context.CancellationToken).Type;
        if (propertyType is null || !IsMutableBclCollection(propertyType))
        {
            return;
        }

        if (property.AccessorList is not { } accessorList)
        {
            return;
        }

        foreach (var accessor in accessorList.Accessors)
        {
            if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration) && accessor.Modifiers.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0051,
                    property.Identifier.GetLocation()));
                return;
            }
        }
    }

    private static bool IsMutableBclCollection(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol)
        {
            return true;
        }

        var definition = type.OriginalDefinition;
        var containingNamespace = definition.ContainingNamespace?.ToDisplayString();
        return containingNamespace is not null
            && CollectionNamespaces.Contains(containingNamespace)
            && CollectionTypeNames.Contains(definition.Name);
    }
}
