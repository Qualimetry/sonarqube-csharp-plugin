using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LocalCanBeConstAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0104);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.LocalDeclarationStatement);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (LocalDeclarationStatementSyntax)context.Node;
        if (declaration.IsConst || !declaration.UsingKeyword.IsKind(SyntaxKind.None))
        {
            return;
        }

        if (declaration.Declaration.Variables.Count != 1)
        {
            return;
        }

        var declarator = declaration.Declaration.Variables[0];
        if (declarator.Initializer is not { } initializer)
        {
            return;
        }

        var model = context.SemanticModel;

        var constant = model.GetConstantValue(initializer.Value);
        if (!constant.HasValue || constant.Value == null)
        {
            return;
        }

        var type = model.GetTypeInfo(declaration.Declaration.Type).Type;
        if (type == null || !IsConstantCompatible(type))
        {
            return;
        }

        if (model.GetDeclaredSymbol(declarator) is not ILocalSymbol local)
        {
            return;
        }

        if (declaration.FirstAncestorOrSelf<BlockSyntax>() is not { } scope)
        {
            return;
        }

        if (IsReassigned(scope, local, model))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0104,
            declarator.Identifier.GetLocation(),
            local.Name));
    }

    private static bool IsConstantCompatible(ITypeSymbol type)
    {
        if (type.TypeKind == TypeKind.Enum)
        {
            return true;
        }

        return type.SpecialType switch
        {
            SpecialType.System_Boolean
                or SpecialType.System_Char
                or SpecialType.System_SByte
                or SpecialType.System_Byte
                or SpecialType.System_Int16
                or SpecialType.System_UInt16
                or SpecialType.System_Int32
                or SpecialType.System_UInt32
                or SpecialType.System_Int64
                or SpecialType.System_UInt64
                or SpecialType.System_Single
                or SpecialType.System_Double
                or SpecialType.System_Decimal
                or SpecialType.System_String => true,
            _ => false,
        };
    }

    private static bool IsReassigned(BlockSyntax scope, ILocalSymbol local, SemanticModel model)
    {
        foreach (var node in scope.DescendantNodes())
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

            if (SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(target).Symbol, local))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsIncrementOrDecrement(SyntaxKind kind) =>
        kind is SyntaxKind.PreIncrementExpression
            or SyntaxKind.PreDecrementExpression
            or SyntaxKind.PostIncrementExpression
            or SyntaxKind.PostDecrementExpression;
}
