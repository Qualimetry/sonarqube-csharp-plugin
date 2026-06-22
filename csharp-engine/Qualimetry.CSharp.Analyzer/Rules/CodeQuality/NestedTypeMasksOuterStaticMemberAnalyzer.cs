using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NestedTypeMasksOuterStaticMemberAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0118);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { ContainingType: { } } typeSymbol)
        {
            return;
        }

        var outerStaticNames = CollectOuterStaticMemberNames(typeSymbol.ContainingType);
        if (outerStaticNames.Count == 0)
        {
            return;
        }

        foreach (var member in declaration.Members)
        {
            foreach (var identifier in MemberIdentifiers(member))
            {
                if (outerStaticNames.Contains(identifier.ValueText))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0118, identifier.GetLocation()));
                }
            }
        }
    }

    private static HashSet<string> CollectOuterStaticMemberNames(INamedTypeSymbol? outer)
    {
        var names = new HashSet<string>();

        for (var current = outer; current is not null; current = current.ContainingType)
        {
            foreach (var member in current.GetMembers())
            {
                if (member.IsStatic && !member.IsImplicitlyDeclared && member.CanBeReferencedByName)
                {
                    names.Add(member.Name);
                }
            }
        }

        return names;
    }

    private static IEnumerable<SyntaxToken> MemberIdentifiers(MemberDeclarationSyntax member)
    {
        switch (member)
        {
            case MethodDeclarationSyntax method:
                yield return method.Identifier;
                break;
            case PropertyDeclarationSyntax property:
                yield return property.Identifier;
                break;
            case EventDeclarationSyntax @event:
                yield return @event.Identifier;
                break;
            case FieldDeclarationSyntax field:
                foreach (var variable in field.Declaration.Variables)
                {
                    yield return variable.Identifier;
                }

                break;
            case EventFieldDeclarationSyntax eventField:
                foreach (var variable in eventField.Declaration.Variables)
                {
                    yield return variable.Identifier;
                }

                break;
        }
    }
}
