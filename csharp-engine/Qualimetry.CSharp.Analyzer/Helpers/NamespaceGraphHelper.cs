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

    /// <summary>
    /// True when one namespace contains the other (e.g. "A.B" and "A.B.C"). References between a
    /// namespace and its own ancestor or descendant are organizational nesting, not a layering
    /// dependency, so they must not count as edges in the cycle graph.
    /// </summary>
    internal static bool IsAncestorOrDescendant(string first, string second)
    {
        if (string.Equals(first, second, StringComparison.Ordinal))
        {
            return true;
        }

        return first.StartsWith(second + ".", StringComparison.Ordinal)
            || second.StartsWith(first + ".", StringComparison.Ordinal);
    }
}
