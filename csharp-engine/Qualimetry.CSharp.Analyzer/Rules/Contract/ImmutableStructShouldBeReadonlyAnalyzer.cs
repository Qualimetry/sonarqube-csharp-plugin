using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Contract;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ImmutableStructShouldBeReadonlyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0095);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (StructDeclarationSyntax)context.Node;

        if (declaration.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)
            || declaration.Modifiers.Any(SyntaxKind.PartialKeyword)
            || declaration.Modifiers.Any(SyntaxKind.RefKeyword))
        {
            return;
        }

        var hasState = false;

        foreach (var member in declaration.Members)
        {
            switch (member)
            {
                case FieldDeclarationSyntax field:
                    if (field.Modifiers.Any(SyntaxKind.StaticKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword))
                    {
                        continue;
                    }

                    if (!field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword))
                    {
                        return;
                    }

                    hasState = true;
                    break;

                case EventFieldDeclarationSyntax eventField:
                    if (!eventField.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        return;
                    }

                    break;

                case PropertyDeclarationSyntax property:
                    if (property.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        continue;
                    }

                    if (HasSetAccessor(property.AccessorList))
                    {
                        return;
                    }

                    hasState = true;
                    break;

                case IndexerDeclarationSyntax indexer:
                    if (HasSetAccessor(indexer.AccessorList))
                    {
                        return;
                    }

                    break;
            }
        }

        if (!hasState)
        {
            return;
        }

        if (AssignsThis(declaration))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0095,
            declaration.Identifier.GetLocation(),
            declaration.Identifier.ValueText));
    }

    private static bool HasSetAccessor(AccessorListSyntax? accessorList)
    {
        return accessorList is not null
            && accessorList.Accessors.Any(accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration));
    }

    private static bool AssignsThis(StructDeclarationSyntax declaration)
    {
        return declaration.DescendantNodes()
            .OfType<AssignmentExpressionSyntax>()
            .Any(assignment => Unwrap(assignment.Left) is ThisExpressionSyntax);
    }

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            expression = parenthesized.Expression;
        }

        return expression;
    }
}
