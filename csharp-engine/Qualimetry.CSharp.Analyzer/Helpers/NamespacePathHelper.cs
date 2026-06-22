using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

    internal static string? GetNamespaceName(INamespaceSymbol? namespaceSymbol)
    {
        if (namespaceSymbol == null || namespaceSymbol.IsGlobalNamespace)
        {
            return null;
        }

        return namespaceSymbol.ToDisplayString();
    }

    /// <summary>
    /// Decides whether a type's namespace disagrees with its folder, expressed relative to the project.
    /// The expected namespace is the project's root namespace followed by the folders between the project
    /// directory and the file, so a file at &lt;ProjectDir&gt;/Foo/Bar.cs in a project rooted at "Acme.Web"
    /// is expected to live in "Acme.Web.Foo". Returns false (no finding) whenever the inputs are missing or
    /// the file sits outside the project directory, because without that context any guess produces noise.
    /// </summary>
    internal static bool TryGetNamespaceMismatch(
        string? namespaceName,
        string? filePath,
        string? projectDirectory,
        string? rootNamespace,
        out string expectedNamespace)
    {
        expectedNamespace = string.Empty;

        if (string.IsNullOrWhiteSpace(namespaceName)
            || string.IsNullOrWhiteSpace(filePath)
            || string.IsNullOrWhiteSpace(projectDirectory)
            || string.IsNullOrWhiteSpace(rootNamespace))
        {
            return false;
        }

        if (!TryGetRelativeFolderSegments(filePath!, projectDirectory!, out var folderSegments))
        {
            return false;
        }

        var expectedSegments = new List<string>(rootNamespace!.Split('.'));
        foreach (var folder in folderSegments)
        {
            var sanitized = SanitizeIdentifier(folder);
            if (string.IsNullOrEmpty(sanitized))
            {
                return false;
            }

            expectedSegments.Add(sanitized);
        }

        var actualSegments = namespaceName!.Split('.');
        if (SegmentsEqual(actualSegments, expectedSegments))
        {
            return false;
        }

        expectedNamespace = string.Join(".", expectedSegments);
        return true;
    }

    internal static string GetProjectRelativeDirectory(string? directory, string? projectDirectory)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(projectDirectory))
        {
            return directory!;
        }

        var normalizedDirectory = TrimTrailingSlash(Normalize(directory!));
        var normalizedProject = TrimTrailingSlash(Normalize(projectDirectory!));
        if (normalizedProject.Length == 0)
        {
            return directory!;
        }

        if (string.Equals(normalizedDirectory, normalizedProject, StringComparison.OrdinalIgnoreCase))
        {
            return ".";
        }

        var prefix = normalizedProject + "/";
        return normalizedDirectory.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? normalizedDirectory.Substring(prefix.Length)
            : directory!;
    }

    internal static bool AreDirectoriesRelatedByPrefix(string first, string second)
    {
        var a = TrimTrailingSlash(Normalize(first));
        var b = TrimTrailingSlash(Normalize(second));
        if (a.Length == 0 || b.Length == 0)
        {
            return true;
        }

        if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return a.StartsWith(b + "/", StringComparison.OrdinalIgnoreCase)
            || b.StartsWith(a + "/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetRelativeFolderSegments(string filePath, string projectDirectory, out IReadOnlyList<string> segments)
    {
        segments = Array.Empty<string>();

        var fileDirectory = GetDirectory(Normalize(filePath));
        var projectDir = TrimTrailingSlash(Normalize(projectDirectory));
        if (projectDir.Length == 0)
        {
            return false;
        }

        if (string.Equals(fileDirectory, projectDir, StringComparison.OrdinalIgnoreCase))
        {
            segments = Array.Empty<string>();
            return true;
        }

        var prefix = projectDir + "/";
        if (!fileDirectory.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        segments = fileDirectory
            .Substring(prefix.Length)
            .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        return true;
    }

    private static bool SegmentsEqual(IReadOnlyList<string> actual, IReadOnlyList<string> expected)
    {
        if (actual.Count != expected.Count)
        {
            return false;
        }

        for (int i = 0; i < actual.Count; i++)
        {
            if (!string.Equals(actual[i], expected[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static string SanitizeIdentifier(string folder)
    {
        var builder = new StringBuilder(folder.Length);
        foreach (var character in folder)
        {
            builder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        var sanitized = builder.ToString();
        if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
        {
            sanitized = "_" + sanitized;
        }

        return sanitized;
    }

    private static string Normalize(string path) => path.Replace('\\', '/').Trim();

    private static string TrimTrailingSlash(string path) => path.TrimEnd('/');

    private static string GetDirectory(string normalizedPath)
    {
        var lastSlash = normalizedPath.LastIndexOf('/');
        return lastSlash <= 0 ? string.Empty : normalizedPath.Substring(0, lastSlash);
    }
}
