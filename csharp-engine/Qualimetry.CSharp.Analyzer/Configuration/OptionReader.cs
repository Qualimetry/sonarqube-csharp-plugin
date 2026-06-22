using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer;

/// <summary>
/// Resolves a configurable rule parameter from the two channels SonarQube and local builds use:
/// the quality-profile value delivered by SonarScanner for .NET in the SonarLint.xml AdditionalFile
/// (connected / CI), falling back to the .editorconfig / .globalconfig value for a local build.
/// </summary>
internal static class OptionReader
{
    private const string OptionPrefix = "qualimetry.";

    private static readonly ConditionalWeakTable<AdditionalText, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> ParsedCache = new();

    public static int ReadInt(SyntaxNodeAnalysisContext context, SyntaxTree tree, string key, int defaultValue)
    {
        var raw = ReadRaw(context, tree, key);
        if (raw != null
            && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            && parsed >= 0)
        {
            return parsed;
        }

        return defaultValue;
    }

    public static string ReadString(SyntaxNodeAnalysisContext context, SyntaxTree tree, string key, string defaultValue)
    {
        var raw = ReadRaw(context, tree, key);
        return string.IsNullOrWhiteSpace(raw) ? defaultValue : raw!;
    }

    private static string? ReadRaw(SyntaxNodeAnalysisContext context, SyntaxTree tree, string key)
    {
        if (TrySplit(key, out var ruleId, out var parameterName))
        {
            var fromProfile = ReadFromSonarLint(context.Options.AdditionalFiles, ruleId, parameterName);
            if (fromProfile != null)
            {
                return fromProfile;
            }
        }

        var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(tree);
        return options.TryGetValue(key, out var raw) ? raw : null;
    }

    private static bool TrySplit(string key, out string ruleId, out string parameterName)
    {
        ruleId = string.Empty;
        parameterName = string.Empty;
        var remainder = key.StartsWith(OptionPrefix, StringComparison.Ordinal) ? key.Substring(OptionPrefix.Length) : key;
        var lastDot = remainder.LastIndexOf('.');
        if (lastDot <= 0 || lastDot == remainder.Length - 1)
        {
            return false;
        }

        ruleId = remainder.Substring(0, lastDot);
        parameterName = remainder.Substring(lastDot + 1);
        return true;
    }

    private static string? ReadFromSonarLint(ImmutableArray<AdditionalText> additionalFiles, string ruleId, string parameterName)
    {
        foreach (var file in additionalFiles)
        {
            if (!IsSonarLintFile(file.Path))
            {
                continue;
            }

            var rules = ParsedCache.GetValue(file, Parse);
            if (rules.TryGetValue(ruleId, out var parameters)
                && parameters.TryGetValue(parameterName, out var value))
            {
                return value;
            }
        }

        return null;
    }

    private static bool IsSonarLintFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        var name = path.Replace('\\', '/');
        var slash = name.LastIndexOf('/');
        if (slash >= 0)
        {
            name = name.Substring(slash + 1);
        }

        return string.Equals(name, "SonarLint.xml", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Parse(AdditionalText file)
    {
        var result = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal);
        var text = file.GetText()?.ToString();
        if (string.IsNullOrEmpty(text))
        {
            return result;
        }

        XDocument document;
        try
        {
            document = XDocument.Parse(text);
        }
        catch (System.Xml.XmlException)
        {
            return result;
        }

        foreach (var rule in document.Descendants("Rule"))
        {
            var ruleKey = rule.Element("Key")?.Value?.Trim();
            if (string.IsNullOrEmpty(ruleKey))
            {
                continue;
            }

            var parameters = new Dictionary<string, string>(StringComparer.Ordinal);
            var parametersElement = rule.Element("Parameters");
            if (parametersElement != null)
            {
                foreach (var parameter in parametersElement.Elements("Parameter"))
                {
                    var paramKey = parameter.Element("Key")?.Value?.Trim();
                    var paramValue = parameter.Element("Value")?.Value;
                    if (!string.IsNullOrEmpty(paramKey) && paramValue != null)
                    {
                        parameters[paramKey!] = paramValue;
                    }
                }
            }

            result[ruleKey!] = parameters;
        }

        return result;
    }
}
