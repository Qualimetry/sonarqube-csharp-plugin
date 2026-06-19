using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TryMethodReturnTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0110);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (method.Modifiers.Any(SyntaxKind.OverrideKeyword))
        {
            return;
        }

        var name = method.Identifier.ValueText;
        if (name.Length <= 3 || !name.StartsWith("Try", System.StringComparison.Ordinal) || !char.IsUpper(name[3]))
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is not { } symbol)
        {
            return;
        }

        if (symbol.IsOverride || IsInterfaceImplementation(symbol))
        {
            return;
        }

        if (IsBooleanLike(symbol.ReturnType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0110, method.Identifier.GetLocation()));
    }

    private static bool IsBooleanLike(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_Boolean)
        {
            return true;
        }

        if (type is INamedTypeSymbol named && named.TypeArguments.Length == 1)
        {
            if (named.Name is "Nullable" or "Task" or "ValueTask"
                && named.TypeArguments[0].SpecialType == SpecialType.System_Boolean)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsInterfaceImplementation(IMethodSymbol method)
    {
        var containingType = method.ContainingType;
        if (containingType is null)
        {
            return false;
        }

        foreach (var @interface in containingType.AllInterfaces)
        {
            foreach (var member in @interface.GetMembers(method.Name))
            {
                if (member is IMethodSymbol interfaceMethod
                    && SymbolEqualityComparer.Default.Equals(
                        containingType.FindImplementationForInterfaceMember(interfaceMethod), method))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
