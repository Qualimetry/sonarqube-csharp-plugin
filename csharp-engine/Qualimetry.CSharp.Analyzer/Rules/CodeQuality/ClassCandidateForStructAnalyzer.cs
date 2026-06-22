using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ClassCandidateForStructAnalyzer : DiagnosticAnalyzer
{
    private const int MaxDataMembers = 3;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0050);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (declaration.Modifiers.Any(SyntaxKind.StaticKeyword)
            || declaration.Modifiers.Any(SyntaxKind.AbstractKeyword)
            || declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        if (declaration.BaseList is not null)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } typeSymbol)
        {
            return;
        }

        var dataMembers = 0;

        foreach (var member in typeSymbol.GetMembers())
        {
            if (member.IsStatic || member.IsImplicitlyDeclared)
            {
                continue;
            }

            switch (member)
            {
                case IFieldSymbol field:
                    if (!field.IsReadOnly || !IsValueLike(field.Type))
                    {
                        return;
                    }

                    dataMembers++;
                    break;
                case IPropertySymbol property:
                    if (property.SetMethod is { IsInitOnly: false } || !IsValueLike(property.Type))
                    {
                        return;
                    }

                    dataMembers++;
                    break;
                case IMethodSymbol method:
                    if (method.MethodKind != MethodKind.Constructor
                        && method.MethodKind != MethodKind.PropertyGet
                        && method.MethodKind != MethodKind.PropertySet)
                    {
                        return;
                    }

                    break;
                default:
                    return;
            }
        }

        if (dataMembers == 0 || dataMembers > MaxDataMembers)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0050, declaration.Identifier.GetLocation()));
    }

    private static bool IsValueLike(ITypeSymbol type)
    {
        return type.IsValueType || type.SpecialType == SpecialType.System_String;
    }
}
