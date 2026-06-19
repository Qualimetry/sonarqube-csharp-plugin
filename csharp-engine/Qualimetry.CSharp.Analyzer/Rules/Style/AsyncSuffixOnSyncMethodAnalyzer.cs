using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncSuffixOnSyncMethodAnalyzer : DiagnosticAnalyzer
{
    private const string Suffix = "Async";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0141);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!method.Identifier.ValueText.EndsWith(Suffix, StringComparison.Ordinal))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method) is not { } symbol)
        {
            return;
        }

        if (symbol.IsAsync || symbol.IsOverride || !symbol.ExplicitInterfaceImplementations.IsEmpty)
        {
            return;
        }

        if (IsAwaitable(symbol.ReturnType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0141, method.Identifier.GetLocation()));
    }

    private static bool IsAwaitable(ITypeSymbol returnType)
    {
        if (returnType.TypeKind == TypeKind.Error)
        {
            return true;
        }

        if (IsAsyncEnumerableLike(returnType))
        {
            return true;
        }

        return returnType
            .GetMembers("GetAwaiter")
            .OfType<IMethodSymbol>()
            .Any(method => method.Parameters.Length == 0 && !method.ReturnsVoid);
    }

    private static bool IsAsyncEnumerableLike(ITypeSymbol type)
    {
        var name = type.OriginalDefinition.ToDisplayString();
        return name == "System.Collections.Generic.IAsyncEnumerable<T>"
            || name == "System.Collections.Generic.IAsyncEnumerator<T>";
    }
}
