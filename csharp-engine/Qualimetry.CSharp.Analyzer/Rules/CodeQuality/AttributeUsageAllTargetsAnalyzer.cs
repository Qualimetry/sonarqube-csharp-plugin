using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeUsageAllTargetsAnalyzer : DiagnosticAnalyzer
{
    private const int AllTargets = 32767;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0166);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration, context.CancellationToken) is not { } typeSymbol
            || !DerivesFromAttribute(typeSymbol))
        {
            return;
        }

        foreach (var attributeList in declaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (!IsAttributeUsage(attribute) || attribute.ArgumentList is null)
                {
                    continue;
                }

                var positional = attribute.ArgumentList.Arguments.FirstOrDefault(a => a.NameEquals is null);
                if (positional is null)
                {
                    continue;
                }

                var constant = context.SemanticModel.GetConstantValue(positional.Expression, context.CancellationToken);
                if (constant.HasValue && constant.Value is int value && value == AllTargets)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0166, attribute.GetLocation()));
                }
            }
        }
    }

    private static bool IsAttributeUsage(AttributeSyntax attribute)
    {
        var name = attribute.Name switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            SimpleNameSyntax simple => simple.Identifier.ValueText,
            _ => string.Empty,
        };

        return name == "AttributeUsage" || name == "AttributeUsageAttribute";
    }

    private static bool DerivesFromAttribute(INamedTypeSymbol typeSymbol)
    {
        for (INamedTypeSymbol? current = typeSymbol.BaseType; current is not null; current = current.BaseType)
        {
            if (current.Name == "Attribute" && current.ContainingNamespace?.ToDisplayString() == "System")
            {
                return true;
            }
        }

        return false;
    }
}
