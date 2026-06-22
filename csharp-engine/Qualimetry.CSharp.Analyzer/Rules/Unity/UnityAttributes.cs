using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.Rules.Unity;

internal static class UnityAttributes
{
    public static bool Has(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
    {
        foreach (var list in attributeLists)
        {
            foreach (var attribute in list.Attributes)
            {
                if (Matches(attribute.Name, attributeName))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool Matches(NameSyntax name, string attributeName)
    {
        var identifier = name switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            SimpleNameSyntax simple => simple.Identifier.ValueText,
            _ => null,
        };

        return identifier == attributeName || identifier == attributeName + "Attribute";
    }
}
