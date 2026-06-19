using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

internal static class UnityTypeNames
{
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
}
