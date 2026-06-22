using System;
using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Style;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RedundantDefaultFieldInitializerAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0139);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;
        if (field.Modifiers.Any(SyntaxKind.ConstKeyword))
        {
            return;
        }

        var model = context.SemanticModel;

        foreach (var declarator in field.Declaration.Variables)
        {
            if (declarator.Initializer is not { } initializer)
            {
                continue;
            }

            if (model.GetDeclaredSymbol(declarator) is not IFieldSymbol fieldSymbol)
            {
                continue;
            }

            if (!IsDefaultValueInitializer(initializer.Value, fieldSymbol.Type, model))
            {
                continue;
            }

            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0139, initializer.Value.GetLocation()));
        }
    }

    private static bool IsDefaultValueInitializer(ExpressionSyntax value, ITypeSymbol type, SemanticModel model)
    {
        if (value is LiteralExpressionSyntax { } literal && literal.IsKind(SyntaxKind.DefaultLiteralExpression))
        {
            return true;
        }

        if (value is DefaultExpressionSyntax)
        {
            return true;
        }

        var constant = model.GetConstantValue(value);
        if (!constant.HasValue)
        {
            return false;
        }

        if (type.IsReferenceType || IsNullableValueType(type))
        {
            return constant.Value == null;
        }

        return IsValueTypeDefault(constant.Value);
    }

    private static bool IsNullableValueType(ITypeSymbol type) =>
        type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

    private static bool IsValueTypeDefault(object? value)
    {
        switch (value)
        {
            case null:
                return false;
            case bool b:
                return !b;
            case char c:
                return c == '\0';
            default:
                try
                {
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture) == 0m;
                }
                catch (Exception)
                {
                    return false;
                }
        }
    }
}
