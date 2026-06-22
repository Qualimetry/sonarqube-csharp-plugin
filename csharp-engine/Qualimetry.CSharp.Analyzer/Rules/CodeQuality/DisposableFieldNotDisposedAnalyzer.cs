using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DisposableFieldNotDisposedAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0061);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;
        var model = context.SemanticModel;

        if (model.GetDeclaredSymbol(declaration) is not { } typeSymbol || !Implements(typeSymbol))
        {
            return;
        }

        var disposedNames = CollectDisposedReceiverNames(declaration);

        foreach (var field in declaration.Members.OfType<FieldDeclarationSyntax>())
        {
            if (field.Modifiers.Any(SyntaxKind.StaticKeyword) || field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                continue;
            }

            foreach (var variable in field.Declaration.Variables)
            {
                if (disposedNames.Contains(variable.Identifier.ValueText))
                {
                    continue;
                }

                if (model.GetDeclaredSymbol(variable) is not IFieldSymbol fieldSymbol || !Implements(fieldSymbol.Type))
                {
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0061,
                    variable.Identifier.GetLocation(),
                    variable.Identifier.ValueText));
            }
        }
    }

    private static bool Implements(ITypeSymbol type)
    {
        if (type.Name == "IDisposable" && type.ContainingNamespace?.ToDisplayString() == "System")
        {
            return true;
        }

        return type.AllInterfaces.Any(i =>
            i.Name == "IDisposable" && i.ContainingNamespace?.ToDisplayString() == "System");
    }

    private static HashSet<string> CollectDisposedReceiverNames(ClassDeclarationSyntax declaration)
    {
        var names = new HashSet<string>();

        foreach (var node in declaration.DescendantNodes())
        {
            switch (node)
            {
                case MemberAccessExpressionSyntax memberAccess
                    when memberAccess.Name.Identifier.ValueText == "Dispose":
                    AddTrailingName(memberAccess.Expression, names);
                    break;

                case MemberBindingExpressionSyntax binding
                    when binding.Name.Identifier.ValueText == "Dispose"
                    && node.Ancestors().OfType<ConditionalAccessExpressionSyntax>().FirstOrDefault() is { } conditional:
                    AddTrailingName(conditional.Expression, names);
                    break;
            }
        }

        return names;
    }

    private static void AddTrailingName(ExpressionSyntax expression, HashSet<string> names)
    {
        switch (expression)
        {
            case IdentifierNameSyntax identifier:
                names.Add(identifier.Identifier.ValueText);
                break;
            case MemberAccessExpressionSyntax memberAccess:
                names.Add(memberAccess.Name.Identifier.ValueText);
                break;
        }
    }
}
