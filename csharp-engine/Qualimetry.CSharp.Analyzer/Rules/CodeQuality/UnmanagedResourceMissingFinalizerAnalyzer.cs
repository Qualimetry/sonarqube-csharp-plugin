using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnmanagedResourceMissingFinalizerAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> HandleTypes = ImmutableHashSet.Create(
        "IntPtr",
        "UIntPtr",
        "nint",
        "nuint");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0059);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (!ImplementsDisposable(declaration))
        {
            return;
        }

        if (declaration.Members.OfType<DestructorDeclarationSyntax>().Any())
        {
            return;
        }

        if (!HasUnmanagedHandleField(declaration))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0059,
            declaration.Identifier.GetLocation()));
    }

    private static bool ImplementsDisposable(ClassDeclarationSyntax declaration)
    {
        if (declaration.BaseList is not { } baseList)
        {
            return false;
        }

        return baseList.Types.Any(t => NameOf(t.Type) == "IDisposable");
    }

    private static bool HasUnmanagedHandleField(ClassDeclarationSyntax declaration)
    {
        foreach (var field in declaration.Members.OfType<FieldDeclarationSyntax>())
        {
            if (field.Modifiers.Any(SyntaxKind.StaticKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                continue;
            }

            if (NameOf(field.Declaration.Type) is { } name && HandleTypes.Contains(name))
            {
                return true;
            }
        }

        return false;
    }

    private static string? NameOf(TypeSyntax type)
    {
        return type switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            GenericNameSyntax generic => generic.Identifier.ValueText,
            PredefinedTypeSyntax predefined => predefined.Keyword.ValueText,
            _ => null,
        };
    }
}
