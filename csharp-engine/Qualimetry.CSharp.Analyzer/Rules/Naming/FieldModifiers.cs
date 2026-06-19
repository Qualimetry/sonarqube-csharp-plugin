using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

internal static class FieldModifiers
{
    public static bool IsPrivate(SyntaxTokenList modifiers)
    {
        return !modifiers.Any(SyntaxKind.PublicKeyword)
            && !modifiers.Any(SyntaxKind.ProtectedKeyword)
            && !modifiers.Any(SyntaxKind.InternalKeyword);
    }
}
