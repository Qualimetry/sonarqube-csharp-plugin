using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SingletonPatternAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0028);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (declaration.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } typeSymbol)
        {
            return;
        }

        if (!HasOnlyPrivateInstanceConstructors(typeSymbol))
        {
            return;
        }

        if (!HasStaticSelfTypedMember(typeSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0028, declaration.Identifier.GetLocation()));
    }

    private static bool HasOnlyPrivateInstanceConstructors(INamedTypeSymbol typeSymbol)
    {
        var instanceConstructors = typeSymbol.InstanceConstructors
            .Where(c => !c.IsImplicitlyDeclared)
            .ToList();

        if (instanceConstructors.Count == 0)
        {
            return false;
        }

        return instanceConstructors.All(c => c.DeclaredAccessibility == Accessibility.Private);
    }

    private static bool HasStaticSelfTypedMember(INamedTypeSymbol typeSymbol)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            if (!member.IsStatic)
            {
                continue;
            }

            var memberType = member switch
            {
                IFieldSymbol field => field.Type,
                IPropertySymbol property => property.Type,
                _ => null,
            };

            if (memberType is not null && SymbolEqualityComparer.Default.Equals(memberType, typeSymbol))
            {
                return true;
            }
        }

        return false;
    }
}
