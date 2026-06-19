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
        "List",
        "Dictionary",
        "HashSet",
        "SortedSet",
        "SortedList",
        "SortedDictionary",
        "Collection",
        "ObservableCollection",
        "ArrayList",
        "Hashtable",
        "Queue",
        "Stack",
        "LinkedList",
        "StringBuilder");

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

        if (!IsMutableCollectionType(field.Declaration.Type))
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0027,
                variable.Identifier.GetLocation()));
        }
    }

    private static bool IsMutableCollectionType(TypeSyntax type)
    {
        switch (type)
        {
            case ArrayTypeSyntax:
                return true;
            case GenericNameSyntax generic:
                return MutableCollectionTypes.Contains(generic.Identifier.ValueText);
            case IdentifierNameSyntax identifier:
                return MutableCollectionTypes.Contains(identifier.Identifier.ValueText);
            case QualifiedNameSyntax qualified:
                return IsMutableCollectionType(qualified.Right);
            default:
                return false;
        }
    }
}
