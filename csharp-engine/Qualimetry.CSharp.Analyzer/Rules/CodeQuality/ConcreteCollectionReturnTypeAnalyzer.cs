using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ConcreteCollectionReturnTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> ConcreteCollections = ImmutableHashSet.Create(
        "System.Collections.Generic.List`1",
        "System.Collections.Generic.Dictionary`2",
        "System.Collections.Generic.HashSet`1",
        "System.Collections.Generic.SortedDictionary`2",
        "System.Collections.Generic.SortedList`2",
        "System.Collections.Generic.SortedSet`1",
        "System.Collections.Generic.Queue`1",
        "System.Collections.Generic.Stack`1",
        "System.Collections.Generic.LinkedList`1",
        "System.Collections.ObjectModel.Collection`1",
        "System.Collections.ObjectModel.ObservableCollection`1");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0129);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!IsExposed(method.Modifiers)
            || method.Modifiers.Any(SyntaxKind.OverrideKeyword)
            || method.ExplicitInterfaceSpecifier is not null
            || !ContainingTypeIsExternallyVisible(context, method))
        {
            return;
        }

        Report(context, method.ReturnType);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;

        if (!IsExposed(property.Modifiers)
            || property.Modifiers.Any(SyntaxKind.OverrideKeyword)
            || property.ExplicitInterfaceSpecifier is not null
            || !ContainingTypeIsExternallyVisible(context, property))
        {
            return;
        }

        Report(context, property.Type);
    }

    private static bool IsExposed(SyntaxTokenList modifiers)
    {
        return modifiers.Any(SyntaxKind.PublicKeyword) || modifiers.Any(SyntaxKind.ProtectedKeyword);
    }

    private static bool ContainingTypeIsExternallyVisible(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax member)
    {
        if (context.SemanticModel.GetDeclaredSymbol(member, context.CancellationToken) is not ISymbol symbol)
        {
            return false;
        }

        for (INamedTypeSymbol? current = symbol.ContainingType; current is not null; current = current.ContainingType)
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

    private static void Report(SyntaxNodeAnalysisContext context, TypeSyntax typeSyntax)
    {
        if (context.SemanticModel.GetTypeInfo(typeSyntax, context.CancellationToken).Type is not INamedTypeSymbol { IsGenericType: true } type)
        {
            return;
        }

        var definition = type.OriginalDefinition;
        var fullName = definition.ContainingNamespace?.ToDisplayString() + "." + definition.MetadataName;
        if (!ConcreteCollections.Contains(fullName))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0129, typeSyntax.GetLocation()));
    }
}
