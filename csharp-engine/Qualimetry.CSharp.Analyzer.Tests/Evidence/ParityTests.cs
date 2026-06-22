using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Qualimetry.CSharp.Analyzer.Rules.Design;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Evidence;

public class ParityTests
{
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "nuget.config")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("repo root not found");
    }

    private static JArray LoadRules(string repoRoot)
    {
        var path = Path.Combine(repoRoot, "csharp-rules", "rules.json");
        return (JArray)JObject.Parse(File.ReadAllText(path))["rules"]!;
    }

    private static Dictionary<string, string> LoadGlobalConfig(string repoRoot)
    {
        var path = Path.Combine(repoRoot, "csharp-engine", "Qualimetry.CSharp.Analyzer", "config", "analyzers.globalconfig");
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in File.ReadAllLines(path))
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith("dotnet_diagnostic.", StringComparison.Ordinal))
            {
                continue;
            }

            var afterPrefix = trimmed.Substring("dotnet_diagnostic.".Length);
            var severityIndex = afterPrefix.LastIndexOf(".severity", StringComparison.Ordinal);
            var id = afterPrefix.Substring(0, severityIndex);
            var token = trimmed.Substring(trimmed.IndexOf('=') + 1).Trim();
            map[id] = token;
        }

        return map;
    }

    private static Dictionary<string, DiagnosticDescriptor> LoadAnalyzerDescriptors()
    {
        var assembly = typeof(AbstractTypePublicConstructorAnalyzer).Assembly;
        var map = new Dictionary<string, DiagnosticDescriptor>(StringComparer.Ordinal);
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer).IsAssignableFrom(type))
            {
                continue;
            }

            if (Activator.CreateInstance(type) is not Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer analyzer)
            {
                continue;
            }

            foreach (var descriptor in analyzer.SupportedDiagnostics)
            {
                map[descriptor.Id] = descriptor;
            }
        }

        return map;
    }

    private static string ExpectedToken(string severity, bool defaultActive)
    {
        if (!defaultActive)
        {
            return "none";
        }

        return severity switch
        {
            "BLOCKER" or "CRITICAL" or "MAJOR" => "warning",
            "MINOR" => "suggestion",
            "INFO" => "silent",
            _ => "warning",
        };
    }

    [Fact]
    public void CatalogueIsTheSingleSourceAcrossChannels()
    {
        var repoRoot = RepoRoot();
        var rules = LoadRules(repoRoot);
        var globalConfig = LoadGlobalConfig(repoRoot);
        var descriptors = LoadAnalyzerDescriptors();

        Assert.Equal(210, rules.Count);
        Assert.Equal(210, globalConfig.Count);

        var problems = new List<string>();
        var defaultActiveCount = 0;

        foreach (var rule in rules.Cast<JObject>())
        {
            var id = (string)rule["id"]!;
            var severity = (string)rule["severity"]!;
            var title = (string)rule["title"]!;
            var defaultActive = (bool)rule["defaultActive"]!;
            var helpUrl = (string)rule["helpUrl"]!;
            if (defaultActive)
            {
                defaultActiveCount++;
            }

            if (!File.Exists(Path.Combine(repoRoot, "csharp-rules", "html", id + ".html")))
            {
                problems.Add(id + ": missing html");
            }

            if (!File.Exists(Path.Combine(repoRoot, "docs", "rules", id + ".md")))
            {
                problems.Add(id + ": missing markdown");
            }

            if (!globalConfig.TryGetValue(id, out var token))
            {
                problems.Add(id + ": missing globalconfig entry");
            }
            else if (token != ExpectedToken(severity, defaultActive))
            {
                problems.Add(id + ": globalconfig token '" + token + "' != expected '" + ExpectedToken(severity, defaultActive) + "'");
            }

            if (!descriptors.TryGetValue(id, out var descriptor))
            {
                problems.Add(id + ": no analyzer descriptor");
            }
            else
            {
                if (descriptor.HelpLinkUri != helpUrl)
                {
                    problems.Add(id + ": descriptor help '" + descriptor.HelpLinkUri + "' != rules.json '" + helpUrl + "'");
                }

                var descriptorTitle = descriptor.Title.ToString();
                if (descriptorTitle != title)
                {
                    problems.Add(id + ": descriptor title '" + descriptorTitle + "' != rules.json '" + title + "'");
                }
            }
        }

        Assert.True(problems.Count == 0, string.Join("; ", problems.Take(20)));
        Assert.Equal(91, defaultActiveCount);
        Assert.Equal(91, globalConfig.Values.Count(v => v != "none"));
        Assert.Equal(210, descriptors.Count);
    }

    [Fact]
    public void EveryConfigurableParameterIsConsumedByAnAnalyzer()
    {
        var repoRoot = RepoRoot();
        var rules = LoadRules(repoRoot);
        var analyzerSource = ReadAnalyzerSource(repoRoot);

        var configurable = rules.Cast<JObject>().Where(r => r["parameters"] != null).ToList();
        Assert.Equal(17, configurable.Count);

        var problems = new List<string>();
        foreach (var rule in configurable)
        {
            var id = (string)rule["id"]!;
            foreach (var param in (JArray)rule["parameters"]!)
            {
                var key = (string)param["key"]!;
                var type = (string)param["type"]!;
                var defaultValue = (string)param["defaultValue"]!;

                if (string.IsNullOrEmpty(defaultValue))
                {
                    problems.Add(id + "/" + key + ": empty default value");
                }

                if (type is not ("INTEGER" or "STRING" or "BOOLEAN" or "TEXT"))
                {
                    problems.Add(id + "/" + key + ": invalid parameter type '" + type + "'");
                }

                var optionKey = "qualimetry." + id + "." + key;
                if (!analyzerSource.Contains(optionKey, StringComparison.Ordinal))
                {
                    problems.Add(id + "/" + key + ": option key '" + optionKey + "' is never read by an analyzer");
                }
            }
        }

        Assert.True(problems.Count == 0, string.Join("; ", problems));
    }

    private static string ReadAnalyzerSource(string repoRoot)
    {
        var dir = Path.Combine(repoRoot, "csharp-engine", "Qualimetry.CSharp.Analyzer", "Rules");
        var builder = new System.Text.StringBuilder();
        foreach (var file in Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
        {
            builder.Append(File.ReadAllText(file));
        }

        return builder.ToString();
    }
}
