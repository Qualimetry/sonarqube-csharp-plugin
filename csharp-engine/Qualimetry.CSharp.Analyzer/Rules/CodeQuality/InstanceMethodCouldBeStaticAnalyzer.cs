using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InstanceMethodCouldBeStaticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0111);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!method.Modifiers.Any(SyntaxKind.PrivateKeyword))
        {
            return;
        }

        if (method.Modifiers.Any(SyntaxKind.StaticKeyword)
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

        if (UsesInstanceState(context, body))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0111, method.Identifier.GetLocation()));
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
