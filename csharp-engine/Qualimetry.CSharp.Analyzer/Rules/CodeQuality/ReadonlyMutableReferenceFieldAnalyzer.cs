using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ReadonlyMutableReferenceFieldAnalyzer : DiagnosticAnalyzer
{
    private static readonly HashSet<string> MutableGenericCollections = new HashSet<string>
    {
        "System.Collections.Generic.List<T>",
        "System.Collections.Generic.Dictionary<TKey, TValue>",
        "System.Collections.Generic.HashSet<T>",
        "System.Collections.Generic.SortedList<TKey, TValue>",
        "System.Collections.Generic.SortedDictionary<TKey, TValue>",
        "System.Collections.Generic.SortedSet<T>",
        "System.Collections.Generic.Queue<T>",
        "System.Collections.Generic.Stack<T>",
        "System.Collections.Generic.LinkedList<T>",
        "System.Collections.ObjectModel.Collection<T>",
        "System.Text.StringBuilder",
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0063);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (!field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            return;
        }

        var fieldType = context.SemanticModel.GetTypeInfo(field.Declaration.Type, context.CancellationToken).Type;
        if (!IsMutableReferenceType(fieldType))
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0063, variable.Identifier.GetLocation()));
        }
    }

    private static bool IsMutableReferenceType(ITypeSymbol? type)
    {
        if (type is null)
        {
            return false;
        }

        if (type.TypeKind == TypeKind.Array)
        {
            return true;
        }

        if (type is INamedTypeSymbol named)
        {
            return MutableGenericCollections.Contains(named.OriginalDefinition.ToDisplayString());
        }

        return false;
    }
}
