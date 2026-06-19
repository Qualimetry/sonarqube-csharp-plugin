using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Qualimetry.CSharp.Analyzer.Helpers;

internal static class NamespacePathHelper
{
    internal static string? GetSourceDirectory(SyntaxTree tree)
    {
        if (string.IsNullOrWhiteSpace(tree.FilePath))
        {
            return null;
        }

        return Path.GetDirectoryName(tree.FilePath);
    }

    internal static IReadOnlyList<string> SplitPathSegments(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Array.Empty<string>();
        }

        return path
            .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    internal static bool NamespaceMatchesDirectory(string? namespaceName, string? directoryPath)
    {
        if (string.IsNullOrWhiteSpace(namespaceName) || string.IsNullOrWhiteSpace(directoryPath))
        {
            return true;
        }

        var namespaceSegments = namespaceName.Split('.');
        var directorySegments = SplitPathSegments(directoryPath);

        if (directorySegments.Count < namespaceSegments.Length)
        {
            return false;
        }

        var directoryTail = directorySegments
            .Skip(directorySegments.Count - namespaceSegments.Length)
            .ToList();

        for (int i = 0; i < namespaceSegments.Length; i++)
        {
            if (!string.Equals(directoryTail[i], namespaceSegments[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    internal static string? GetNamespaceName(INamespaceSymbol? namespaceSymbol)
    {
        if (namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace)
        {
            return null;
        }

        return namespaceSymbol.ToDisplayString();
    }
}
