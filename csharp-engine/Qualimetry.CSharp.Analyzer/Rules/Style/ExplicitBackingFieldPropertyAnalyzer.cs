using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExplicitBackingFieldPropertyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0170);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.PropertyDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;
        if (property.AccessorList == null)
        {
            return;
        }

        AccessorDeclarationSyntax? getter = null;
        AccessorDeclarationSyntax? setter = null;
        foreach (var accessor in property.AccessorList.Accessors)
        {
            if (accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
            {
                getter = accessor;
            }
            else if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
            {
                setter = accessor;
            }
            else
            {
                return;
            }
        }

        if (getter == null || setter == null)
        {
            return;
        }

        var getField = GetReturnedFieldName(getter);
        if (getField == null)
        {
            return;
        }

        var setField = GetAssignedFieldName(setter);
        if (setField == null || setField != getField)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0170, property.Identifier.GetLocation()));
    }

    private static string? GetReturnedFieldName(AccessorDeclarationSyntax accessor)
    {
        ExpressionSyntax? expression = null;
        if (accessor.ExpressionBody != null)
        {
            expression = accessor.ExpressionBody.Expression;
        }
        else if (accessor.Body != null && accessor.Body.Statements.Count == 1
            && accessor.Body.Statements[0] is ReturnStatementSyntax returnStatement)
        {
            expression = returnStatement.Expression;
        }

        return GetSimpleName(expression);
    }

    private static string? GetAssignedFieldName(AccessorDeclarationSyntax accessor)
    {
        ExpressionSyntax? expression = null;
        if (accessor.ExpressionBody != null)
        {
            expression = accessor.ExpressionBody.Expression;
        }
        else if (accessor.Body != null && accessor.Body.Statements.Count == 1
            && accessor.Body.Statements[0] is ExpressionStatementSyntax expressionStatement)
        {
            expression = expressionStatement.Expression;
        }

        if (!(expression is AssignmentExpressionSyntax assignment) || !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression))
        {
            return null;
        }

        if (!(assignment.Right is IdentifierNameSyntax right) || right.Identifier.ValueText != "value")
        {
            return null;
        }

        return GetSimpleName(assignment.Left);
    }

    private static string? GetSimpleName(ExpressionSyntax? expression)
    {
        switch (expression)
        {
            case IdentifierNameSyntax identifier:
                return identifier.Identifier.ValueText;
            case MemberAccessExpressionSyntax memberAccess when memberAccess.Expression is ThisExpressionSyntax:
                return memberAccess.Name.Identifier.ValueText;
            default:
                return null;
        }
    }
}
