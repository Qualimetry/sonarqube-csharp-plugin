using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

internal static class ExceptionTypeName
{
    public static string? Of(TypeSyntax type)
    {
        return type switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            GenericNameSyntax generic => generic.Identifier.ValueText,
            _ => null,
        };
    }
}
