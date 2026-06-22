using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DisposableFieldInNonDisposableTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0163);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } typeSymbol)
        {
            return;
        }

        if (IsDisposable(typeSymbol))
        {
            return;
        }

        var candidateFields = CollectDisposableInstanceFields(context, declaration);
        if (candidateFields.Count == 0)
        {
            return;
        }

        if (candidateFields.Any(field => OwnsByInitializer(context, field) || OwnsByConstructorAssignment(context, declaration, field)))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0163, declaration.Identifier.GetLocation()));
        }
    }

    private static List<IFieldSymbol> CollectDisposableInstanceFields(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax declaration)
    {
        var result = new List<IFieldSymbol>();

        foreach (var field in declaration.Members.OfType<FieldDeclarationSyntax>())
        {
            if (field.Modifiers.Any(SyntaxKind.StaticKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                continue;
            }

            var fieldType = context.SemanticModel.GetTypeInfo(field.Declaration.Type, context.CancellationToken).Type;
            if (!IsDisposable(fieldType))
            {
                continue;
            }

            foreach (var variable in field.Declaration.Variables)
            {
                if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is IFieldSymbol fieldSymbol)
                {
                    result.Add(fieldSymbol);
                }
            }
        }

        return result;
    }

    private static bool OwnsByInitializer(SyntaxNodeAnalysisContext context, IFieldSymbol field)
    {
        foreach (var reference in field.DeclaringSyntaxReferences)
        {
            if (reference.GetSyntax(context.CancellationToken) is VariableDeclaratorSyntax variable
                && IsObjectCreation(variable.Initializer?.Value))
            {
                return true;
            }
        }

        return false;
    }

    private static bool OwnsByConstructorAssignment(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax declaration, IFieldSymbol field)
    {
        foreach (var constructor in declaration.Members.OfType<ConstructorDeclarationSyntax>())
        {
            foreach (var assignment in constructor.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                if (!IsObjectCreation(assignment.Right))
                {
                    continue;
                }

                var assignedSymbol = context.SemanticModel.GetSymbolInfo(assignment.Left, context.CancellationToken).Symbol;
                if (SymbolEqualityComparer.Default.Equals(assignedSymbol, field))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsObjectCreation(ExpressionSyntax? expression)
        => expression is ObjectCreationExpressionSyntax or ImplicitObjectCreationExpressionSyntax;

    private static bool IsDisposable(ITypeSymbol? type)
    {
        if (type is null)
        {
            return false;
        }

        if (IsDisposableInterface(type))
        {
            return true;
        }

        return type.AllInterfaces.Any(IsDisposableInterface);
    }

    private static bool IsDisposableInterface(ITypeSymbol type)
    {
        return (type.Name == "IDisposable" || type.Name == "IAsyncDisposable")
            && type.ContainingNamespace?.ToDisplayString() == "System";
    }
}
