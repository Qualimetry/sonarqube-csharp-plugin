using System;
using Microsoft.CodeAnalysis;

namespace Qualimetry.CSharp.Analyzer.Helpers;

internal static class NamespaceGraphHelper
{
    internal static bool ShouldIncludeNamespace(string? namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            return false;
        }

        if (namespaceName.StartsWith("System", StringComparison.Ordinal)
            || namespaceName.StartsWith("Microsoft", StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    internal static string? GetNamespaceName(INamespaceSymbol? namespaceSymbol)
    {
        return NamespacePathHelper.GetNamespaceName(namespaceSymbol);
    }
}
