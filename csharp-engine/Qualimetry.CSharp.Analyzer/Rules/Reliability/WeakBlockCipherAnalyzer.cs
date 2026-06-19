using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WeakBlockCipherAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> WeakCipherTypes = ImmutableHashSet.Create(
        "DES",
        "TripleDES",
        "DESCryptoServiceProvider",
        "TripleDESCryptoServiceProvider",
        "TripleDESCng");

    private static readonly ImmutableHashSet<string> WeakFactoryNames = ImmutableHashSet.Create(
        "DES",
        "TripleDES",
        "3DES");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0081);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeCreation, SyntaxKind.ObjectCreationExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeCreation(SyntaxNodeAnalysisContext context)
    {
        var creation = (ObjectCreationExpressionSyntax)context.Node;
        var name = SimpleTypeName(creation.Type);
        if (name is not null && WeakCipherTypes.Contains(name))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0081, creation.Type.GetLocation()));
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess
            || memberAccess.Name.Identifier.ValueText != "Create")
        {
            return;
        }

        var receiver = SimpleTypeName(memberAccess.Expression);
        if (receiver is not null && WeakCipherTypes.Contains(receiver))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0081, memberAccess.GetLocation()));
            return;
        }

        if (receiver is "SymmetricAlgorithm" or "CryptoConfig"
            && invocation.ArgumentList.Arguments.Count > 0
            && invocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.StringLiteralExpression)
            && WeakFactoryNames.Contains(literal.Token.ValueText))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0081, memberAccess.GetLocation()));
        }
    }

    private static string? SimpleTypeName(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
            GenericNameSyntax generic => generic.Identifier.ValueText,
            _ => null,
        };
    }
}
