using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MutableStructAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0156);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (StructDeclarationSyntax)context.Node;

        if (declaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
        {
            return;
        }

        var structSymbol = context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken);

        foreach (var member in declaration.Members)
        {
            if (member.Modifiers.Any(SyntaxKind.StaticKeyword) || member.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                continue;
            }

            if (HasMutableState(context, structSymbol, member))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0156, declaration.Identifier.GetLocation()));
                return;
            }
        }
    }

    private static bool HasMutableState(SyntaxNodeAnalysisContext context, INamedTypeSymbol? structSymbol, MemberDeclarationSyntax member)
    {
        switch (member)
        {
            case PropertyDeclarationSyntax property when property.AccessorList is not null:
                foreach (var accessor in property.AccessorList.Accessors)
                {
                    if (!accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                    {
                        continue;
                    }

                    var propertySymbol = context.SemanticModel.GetDeclaredSymbol(property, context.CancellationToken);
                    if (propertySymbol?.SetMethod is { } setter
                        && structSymbol is not null
                        && setter.DeclaredAccessibility < structSymbol.DeclaredAccessibility)
                    {
                        continue;
                    }

                    return true;
                }

                return false;
            case FieldDeclarationSyntax field:
                return !field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
                    && (field.Modifiers.Any(SyntaxKind.PublicKeyword) || field.Modifiers.Any(SyntaxKind.InternalKeyword));
            default:
                return false;
        }
    }
}
