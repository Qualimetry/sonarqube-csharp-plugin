using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UninstantiableClassAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0120);

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

        if (declaration.Parent is TypeDeclarationSyntax)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } symbol)
        {
            return;
        }

        var declaredConstructors = symbol.InstanceConstructors
            .Where(c => !c.IsImplicitlyDeclared)
            .ToList();

        if (declaredConstructors.Count == 0
            || declaredConstructors.Any(c => c.DeclaredAccessibility != Accessibility.Private))
        {
            return;
        }

        if (symbol.GetMembers().Any(m => m.IsStatic && !m.IsImplicitlyDeclared))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0120, declaration.Identifier.GetLocation()));
    }
}
