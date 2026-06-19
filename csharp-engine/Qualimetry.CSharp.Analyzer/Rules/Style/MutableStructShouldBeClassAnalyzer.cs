using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MutableStructShouldBeClassAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0190);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var structDeclaration = (StructDeclarationSyntax)context.Node;

        if (structDeclaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
            || structDeclaration.Modifiers.Any(SyntaxKind.RefKeyword))
        {
            return;
        }

        if (!HasPubliclyMutableMember(structDeclaration))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0190,
            structDeclaration.Identifier.GetLocation()));
    }

    private static bool HasPubliclyMutableMember(StructDeclarationSyntax structDeclaration)
    {
        foreach (var member in structDeclaration.Members)
        {
            switch (member)
            {
                case FieldDeclarationSyntax field when IsMutablePublicField(field):
                    return true;
                case PropertyDeclarationSyntax property when IsPubliclySettableProperty(property):
                    return true;
            }
        }

        return false;
    }

    private static bool IsMutablePublicField(FieldDeclarationSyntax field)
    {
        if (!field.Modifiers.Any(SyntaxKind.PublicKeyword))
        {
            return false;
        }

        return !field.Modifiers.Any(SyntaxKind.StaticKeyword)
            && !field.Modifiers.Any(SyntaxKind.ConstKeyword)
            && !field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
    }

    private static bool IsPubliclySettableProperty(PropertyDeclarationSyntax property)
    {
        if (!property.Modifiers.Any(SyntaxKind.PublicKeyword)
            || property.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return false;
        }

        if (property.AccessorList is not { } accessorList)
        {
            return false;
        }

        foreach (var accessor in accessorList.Accessors)
        {
            if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration) && accessor.Modifiers.Count == 0)
            {
                return true;
            }
        }

        return false;
    }
}
