using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

internal static class UnityTypeNames
{
    private const string UnityEngineNamespace = "UnityEngine";

    public static string? SimpleName(TypeSyntax type)
    {
        return type switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            GenericNameSyntax generic => generic.Identifier.ValueText,
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            _ => null,
        };
    }

    public static bool IsUnityType(ITypeSymbol? type, string unityTypeName)
    {
        return type is not null
            && type.Name == unityTypeName
            && type.ContainingNamespace is { Name: UnityEngineNamespace, ContainingNamespace.IsGlobalNamespace: true };
    }

    public static bool DerivesFrom(ITypeSymbol? type, string unityTypeName)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (IsUnityType(current, unityTypeName))
            {
                return true;
            }
        }

        return false;
    }
}
