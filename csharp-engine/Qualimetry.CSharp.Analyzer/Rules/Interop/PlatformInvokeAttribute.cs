using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Qualimetry.CSharp.Analyzer.Rules.Interop;

internal static class PlatformInvokeAttribute
{
    public static bool IsPresent(MethodDeclarationSyntax method, SemanticModel semanticModel)
    {
        foreach (var attributeList in method.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (semanticModel.GetSymbolInfo(attribute).Symbol is IMethodSymbol constructor
                    && IsPlatformInvokeAttribute(constructor.ContainingType))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsPlatformInvokeAttribute(INamedTypeSymbol type)
    {
        return type.ToDisplayString() is "System.Runtime.InteropServices.DllImportAttribute"
            or "System.Runtime.InteropServices.LibraryImportAttribute";
    }
}
