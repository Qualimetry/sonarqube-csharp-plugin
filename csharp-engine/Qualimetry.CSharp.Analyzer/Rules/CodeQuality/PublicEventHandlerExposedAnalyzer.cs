using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PublicEventHandlerExposedAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0150);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!method.Modifiers.Any(SyntaxKind.PublicKeyword)
            || method.Modifiers.Any(SyntaxKind.OverrideKeyword)
            || method.ExplicitInterfaceSpecifier is not null)
        {
            return;
        }

        if (!FollowsHandlerNaming(method.Identifier.ValueText))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is not { } symbol
            || !symbol.ReturnsVoid
            || symbol.Parameters.Length != 2
            || symbol.ContainingType?.TypeKind == TypeKind.Interface)
        {
            return;
        }

        if (symbol.Parameters[0].Type.SpecialType != SpecialType.System_Object
            || !IsEventArgs(symbol.Parameters[1].Type))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0150, method.Identifier.GetLocation()));
    }

    private static bool FollowsHandlerNaming(string name)
    {
        if (name.Contains('_')
            || name.EndsWith("Handler", System.StringComparison.Ordinal)
            || name.EndsWith("Callback", System.StringComparison.Ordinal))
        {
            return true;
        }

        return name.Length > 2 && name[0] == 'O' && name[1] == 'n' && char.IsUpper(name[2]);
    }

    private static bool IsEventArgs(ITypeSymbol type)
    {
        for (ITypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (current.Name == "EventArgs" && current.ContainingNamespace?.ToDisplayString() == "System")
            {
                return true;
            }
        }

        return false;
    }
}
