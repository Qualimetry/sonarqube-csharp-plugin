using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FieldCanBeReadonlyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0103);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.RecordStructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var type = (TypeDeclarationSyntax)context.Node;
        if (type.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        var model = context.SemanticModel;

        foreach (var field in type.Members.OfType<FieldDeclarationSyntax>())
        {
            if (!IsCandidate(field) || field.Declaration.Variables.Count != 1)
            {
                continue;
            }

            var declarator = field.Declaration.Variables[0];
            if (model.GetDeclaredSymbol(declarator) is not IFieldSymbol fieldSymbol || fieldSymbol.IsStatic)
            {
                continue;
            }

            var classification = ClassifyWrites(type, fieldSymbol, model);
            if (classification == WriteClassification.Disqualified)
            {
                continue;
            }

            var hasValue = declarator.Initializer != null || classification == WriteClassification.ConstructorOnly;
            if (!hasValue)
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0103,
                declarator.Identifier.GetLocation(),
                fieldSymbol.Name));
        }
    }

    private enum WriteClassification
    {
        None,
        ConstructorOnly,
        Disqualified,
    }

    private static bool IsCandidate(FieldDeclarationSyntax field)
    {
        var modifiers = field.Modifiers;
        if (modifiers.Any(SyntaxKind.ReadOnlyKeyword)
            || modifiers.Any(SyntaxKind.ConstKeyword)
            || modifiers.Any(SyntaxKind.VolatileKeyword)
            || modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return false;
        }

        return !modifiers.Any(SyntaxKind.PublicKeyword)
            && !modifiers.Any(SyntaxKind.InternalKeyword)
            && !modifiers.Any(SyntaxKind.ProtectedKeyword);
    }

    private static WriteClassification ClassifyWrites(TypeDeclarationSyntax type, IFieldSymbol fieldSymbol, SemanticModel model)
    {
        var result = WriteClassification.None;

        foreach (var node in type.DescendantNodes())
        {
            ExpressionSyntax? target = node switch
            {
                AssignmentExpressionSyntax assignment => assignment.Left,
                PrefixUnaryExpressionSyntax prefix when IsIncrementOrDecrement(prefix.Kind()) => prefix.Operand,
                PostfixUnaryExpressionSyntax postfix when IsIncrementOrDecrement(postfix.Kind()) => postfix.Operand,
                ArgumentSyntax argument when !argument.RefKindKeyword.IsKind(SyntaxKind.None) => argument.Expression,
                _ => null,
            };

            if (target == null)
            {
                continue;
            }

            if (!SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(target).Symbol, fieldSymbol))
            {
                continue;
            }

            if (!IsConstructorWrite(target))
            {
                return WriteClassification.Disqualified;
            }

            result = WriteClassification.ConstructorOnly;
        }

        return result;
    }

    private static bool IsIncrementOrDecrement(SyntaxKind kind) =>
        kind is SyntaxKind.PreIncrementExpression
            or SyntaxKind.PreDecrementExpression
            or SyntaxKind.PostIncrementExpression
            or SyntaxKind.PostDecrementExpression;

    private static bool IsConstructorWrite(SyntaxNode node)
    {
        for (SyntaxNode? current = node; current != null; current = current.Parent)
        {
            switch (current)
            {
                case SimpleLambdaExpressionSyntax:
                case ParenthesizedLambdaExpressionSyntax:
                case AnonymousMethodExpressionSyntax:
                case LocalFunctionStatementSyntax:
                    return false;
                case ConstructorDeclarationSyntax constructor:
                    return !constructor.Modifiers.Any(SyntaxKind.StaticKeyword);
                case BaseMethodDeclarationSyntax:
                case AccessorDeclarationSyntax:
                case BasePropertyDeclarationSyntax:
                case TypeDeclarationSyntax:
                    return false;
            }
        }

        return false;
    }
}
