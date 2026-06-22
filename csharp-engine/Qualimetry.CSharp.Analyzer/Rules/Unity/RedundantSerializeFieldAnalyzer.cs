using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RedundantSerializeFieldAnalyzer : DiagnosticAnalyzer
{
    private const string AttributeName = "SerializeField";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0157);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.FieldDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (!field.Modifiers.Any(SyntaxKind.PublicKeyword))
        {
            return;
        }

        var attribute = FindSerializeField(field);
        if (attribute is null)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(attribute).Symbol?.ContainingType is not INamedTypeSymbol attributeType
            || !UnityTypeNames.IsUnityType(attributeType, AttributeName))
        {
            return;
        }

        if (field.Parent is not TypeDeclarationSyntax typeDeclaration
            || context.SemanticModel.GetDeclaredSymbol(typeDeclaration) is not INamedTypeSymbol declaringType
            || !IsUnitySerializableType(declaringType))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0157, attribute.GetLocation()));
    }

    private static bool IsUnitySerializableType(INamedTypeSymbol type)
    {
        return UnityTypeNames.DerivesFrom(type, "MonoBehaviour")
            || UnityTypeNames.DerivesFrom(type, "ScriptableObject");
    }

    private static AttributeSyntax? FindSerializeField(FieldDeclarationSyntax field)
    {
        foreach (var list in field.AttributeLists)
        {
            foreach (var attribute in list.Attributes)
            {
                var identifier = attribute.Name switch
                {
                    QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
                    SimpleNameSyntax simple => simple.Identifier.ValueText,
                    _ => null,
                };

                if (identifier == AttributeName || identifier == AttributeName + "Attribute")
                {
                    return attribute;
                }
            }
        }

        return null;
    }
}
