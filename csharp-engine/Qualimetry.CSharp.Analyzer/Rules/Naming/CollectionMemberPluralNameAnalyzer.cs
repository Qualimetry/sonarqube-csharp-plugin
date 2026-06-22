using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CollectionMemberPluralNameAnalyzer : DiagnosticAnalyzer
{
    private static readonly HashSet<string> AlreadyPluralOrUncountableNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Children",
        "People",
        "Men",
        "Women",
        "Data",
        "Metadata",
        "Media",
        "Series",
        "Species",
        "Status",
        "Information",
        "Content",
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0131);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(property) is not IPropertySymbol symbol)
        {
            return;
        }

        if (symbol.DeclaredAccessibility != Accessibility.Public)
        {
            return;
        }

        Report(context, symbol.Type, property.Identifier);
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        foreach (var variable in field.Declaration.Variables)
        {
            if (context.SemanticModel.GetDeclaredSymbol(variable) is not IFieldSymbol symbol)
            {
                continue;
            }

            if (symbol.DeclaredAccessibility != Accessibility.Public)
            {
                continue;
            }

            Report(context, symbol.Type, variable.Identifier);
        }
    }

    private static void Report(SyntaxNodeAnalysisContext context, ITypeSymbol type, SyntaxToken identifier)
    {
        if (!IsCollection(type))
        {
            return;
        }

        var name = identifier.ValueText;
        if (name.Length == 0 || name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (AlreadyPluralOrUncountableNames.Contains(name))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0131, identifier.GetLocation(), name));
    }

    private static bool IsCollection(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_String)
        {
            return false;
        }

        if (type.TypeKind == TypeKind.Array)
        {
            return true;
        }

        if (type.SpecialType == SpecialType.System_Collections_IEnumerable)
        {
            return true;
        }

        foreach (var iface in type.AllInterfaces)
        {
            if (iface.SpecialType == SpecialType.System_Collections_IEnumerable)
            {
                return true;
            }
        }

        return false;
    }
}
