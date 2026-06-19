using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NonPrivateMethodCouldBeStaticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0177);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        var isExposed = method.Modifiers.Any(SyntaxKind.PublicKeyword)
            || method.Modifiers.Any(SyntaxKind.InternalKeyword)
            || (method.Modifiers.Any(SyntaxKind.ProtectedKeyword) && !method.Modifiers.Any(SyntaxKind.PrivateKeyword));

        if (!isExposed
            || method.ExplicitInterfaceSpecifier is not null
            || method.Modifiers.Any(SyntaxKind.StaticKeyword)
            || method.Modifiers.Any(SyntaxKind.VirtualKeyword)
            || method.Modifiers.Any(SyntaxKind.AbstractKeyword)
            || method.Modifiers.Any(SyntaxKind.OverrideKeyword)
            || method.Modifiers.Any(SyntaxKind.ExternKeyword)
            || method.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        SyntaxNode? body = (SyntaxNode?)method.Body ?? method.ExpressionBody;
        if (body is null)
        {
            return;
        }

        if (context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken) is not { } symbol
            || ImplementsInterface(symbol))
        {
            return;
        }

        if (UsesInstanceState(context, body))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0177, method.Identifier.GetLocation()));
    }

    private static bool ImplementsInterface(IMethodSymbol symbol)
    {
        var containingType = symbol.ContainingType;
        if (containingType is null)
        {
            return false;
        }

        foreach (var iface in containingType.AllInterfaces)
        {
            foreach (var member in iface.GetMembers())
            {
                if (SymbolEqualityComparer.Default.Equals(containingType.FindImplementationForInterfaceMember(member), symbol))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool UsesInstanceState(SyntaxNodeAnalysisContext context, SyntaxNode body)
    {
        foreach (var node in body.DescendantNodesAndSelf())
        {
            if (node is ThisExpressionSyntax or BaseExpressionSyntax)
            {
                return true;
            }

            if (node is not IdentifierNameSyntax identifier)
            {
                continue;
            }

            if (identifier.Parent is MemberAccessExpressionSyntax memberAccess && memberAccess.Name == identifier)
            {
                continue;
            }

            var info = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken);
            var symbol = info.Symbol;
            if (symbol is null)
            {
                if (info.CandidateSymbols.Length == 0)
                {
                    continue;
                }

                return true;
            }

            switch (symbol)
            {
                case IParameterSymbol:
                case ILocalSymbol:
                case ITypeSymbol:
                case INamespaceSymbol:
                    continue;
                case IMethodSymbol { MethodKind: MethodKind.LocalFunction }:
                    continue;
                case IFieldSymbol { IsStatic: false }:
                case IPropertySymbol { IsStatic: false }:
                case IEventSymbol { IsStatic: false }:
                case IMethodSymbol { IsStatic: false }:
                    return true;
                default:
                    continue;
            }
        }

        return false;
    }
}
