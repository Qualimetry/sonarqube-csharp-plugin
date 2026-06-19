using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DisposableFieldInNonDisposableTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0163);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } typeSymbol)
        {
            return;
        }

        if (IsDisposable(typeSymbol))
        {
            return;
        }

        foreach (var field in declaration.Members.OfType<FieldDeclarationSyntax>())
        {
            if (field.Modifiers.Any(SyntaxKind.StaticKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                continue;
            }

            var fieldType = context.SemanticModel.GetTypeInfo(field.Declaration.Type, context.CancellationToken).Type;
            if (IsDisposable(fieldType))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0163, declaration.Identifier.GetLocation()));
                return;
            }
        }
    }

    private static bool IsDisposable(ITypeSymbol? type)
    {
        if (type is null)
        {
            return false;
        }

        if (IsDisposableInterface(type))
        {
            return true;
        }

        return type.AllInterfaces.Any(IsDisposableInterface);
    }

    private static bool IsDisposableInterface(ITypeSymbol type)
    {
        return (type.Name == "IDisposable" || type.Name == "IAsyncDisposable")
            && type.ContainingNamespace?.ToDisplayString() == "System";
    }
}
