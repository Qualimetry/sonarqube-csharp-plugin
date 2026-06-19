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
    private static readonly ImmutableHashSet<string> CollectionTypes = ImmutableHashSet.Create(
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

        if (!IsCollectionType(property.Type))
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

    private static bool IsCollectionType(TypeSyntax type)
    {
        switch (type)
        {
            case ArrayTypeSyntax:
                return true;
            case GenericNameSyntax generic:
                return CollectionTypes.Contains(generic.Identifier.ValueText);
            case IdentifierNameSyntax identifier:
                return CollectionTypes.Contains(identifier.Identifier.ValueText);
            case QualifiedNameSyntax qualified:
                return IsCollectionType(qualified.Right);
            default:
                return false;
        }
    }
}
