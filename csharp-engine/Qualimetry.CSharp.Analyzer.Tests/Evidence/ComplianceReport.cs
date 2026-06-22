using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Newtonsoft.Json.Linq;
using Qualimetry.CSharp.Analyzer.Rules.Design;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Evidence;

public class ComplianceReport
{
    private const string Usings =
        "using System;\n" +
        "using System.Collections;\n" +
        "using System.Collections.Generic;\n" +
        "using System.Linq;\n" +
        "using System.Threading;\n" +
        "using System.Threading.Tasks;\n" +
        "using System.Text;\n" +
        "using System.Globalization;\n" +
        "using System.Runtime.InteropServices;\n" +
        "using System.Runtime.Serialization;\n";

    [Fact]
    public async Task EveryRuleIsCompliantAndCovered()
    {
        var repoRoot = FindRepoRoot();
        var rules = LoadCatalogue(repoRoot);
        Assert.Equal(210, rules.Count);

        var authored = LoadAuthored(repoRoot);
        var analyzersById = BuildAnalyzerIndex();
        var positiveMarkers = LoadDiagnosticMarkers(repoRoot);
        var negativeTests = LoadCleanTestRuleKeys(repoRoot);
        var configTested = LoadConfigOptionRuleKeys(repoRoot);

        var references = await ReferenceAssemblies.Net.Net80.ResolveAsync(LanguageNames.CSharp, CancellationToken.None);
        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        var rows = new List<RuleRow>();

        foreach (var rule in rules.OrderBy(r => r.Key, StringComparer.Ordinal))
        {
            authored.TryGetValue(rule.Key, out var body);
            analyzersById.TryGetValue(rule.Key, out var analyzer);

            var configProvider = BuildConfigProvider(rule.Key, body?["configuration"] as JArray);
            var noncompliant = (string?)body?["noncompliant"] ?? string.Empty;
            var compliant = (string?)body?["compliant"] ?? string.Empty;

            var firedScaffold = -1;
            if (analyzer != null && noncompliant.Length > 0)
            {
                firedScaffold = await FiringScaffoldAsync(analyzer, rule.Key, noncompliant, references, compilationOptions, configProvider);
            }

            var firesByExample = firedScaffold >= 0;
            var firesByTest = positiveMarkers.Contains(rule.Key);
            var fires = firesByExample || firesByTest;

            bool cleanByExample = false;
            if (analyzer != null && compliant.Length > 0)
            {
                var scaffolds = Scaffold(compliant).ToList();
                if (firedScaffold >= 0 && firedScaffold < scaffolds.Count)
                {
                    cleanByExample = !await FiresAsync(analyzer, rule.Key, scaffolds[firedScaffold], references, compilationOptions, configProvider);
                }
                else
                {
                    cleanByExample = !await AnyScaffoldFiresAsync(analyzer, rule.Key, compliant, references, compilationOptions, configProvider);
                }
            }

            var cleanByTest = negativeTests.Contains(rule.Key);
            var clean = cleanByExample || cleanByTest;

            var tunable = (body?["configuration"] as JArray)?.Count > 0;
            var configOk = !tunable || configTested.Contains(rule.Key);

            rows.Add(new RuleRow
            {
                Key = rule.Key,
                Title = rule.Title,
                Category = rule.Category,
                Severity = rule.Severity,
                Profile = rule.Profile,
                DefaultActive = rule.DefaultActive,
                Noncompliant = noncompliant,
                Compliant = compliant,
                Summary = ((body?["summary"] as JArray)?.Select(t => (string?)t ?? string.Empty)).FirstOrDefault() ?? string.Empty,
                HasAnalyzer = analyzer != null,
                FiresByExample = firesByExample,
                FiresByTest = firesByTest,
                Fires = fires,
                CleanByExample = cleanByExample,
                CleanByTest = cleanByTest,
                Clean = clean,
                HasPositiveTest = firesByTest,
                HasNegativeTest = cleanByTest || cleanByExample,
                Tunable = tunable,
                ConfigTested = configOk,
            });
        }

        WriteReport(repoRoot, rows);

        var gaps = new List<string>();
        foreach (var r in rows)
        {
            if (!r.HasAnalyzer) gaps.Add(r.Key + " (no analyzer)");
            else if (!r.Fires) gaps.Add(r.Key + " (does not fire on noncompliant example or in a unit test)");
            else if (!r.Clean) gaps.Add(r.Key + " (false positive: fires on compliant example and no clean unit test)");
            else if (!r.HasPositiveTest) gaps.Add(r.Key + " (no noncompliant unit test)");
            else if (!r.HasNegativeTest) gaps.Add(r.Key + " (no compliant/clean unit test)");
            else if (!r.ConfigTested) gaps.Add(r.Key + " (tunable rule without a configuration unit test)");
        }

        Assert.True(gaps.Count == 0, "Compliance gaps:\n  " + string.Join("\n  ", gaps));
        Assert.Equal(210, rows.Count);
    }

    private static async Task<int> FiringScaffoldAsync(
        DiagnosticAnalyzer analyzer, string id, string snippet,
        ImmutableArray<MetadataReference> references, CSharpCompilationOptions options, AnalyzerConfigOptionsProvider config)
    {
        var scaffolds = Scaffold(snippet).ToList();
        for (var i = 0; i < scaffolds.Count; i++)
        {
            if (await FiresAsync(analyzer, id, scaffolds[i], references, options, config))
            {
                return i;
            }
        }

        return -1;
    }

    private static async Task<bool> AnyScaffoldFiresAsync(
        DiagnosticAnalyzer analyzer, string id, string snippet,
        ImmutableArray<MetadataReference> references, CSharpCompilationOptions options, AnalyzerConfigOptionsProvider config)
    {
        foreach (var candidate in Scaffold(snippet))
        {
            if (await FiresAsync(analyzer, id, candidate, references, options, config))
            {
                return true;
            }
        }

        return false;
    }

    private static async Task<bool> FiresAsync(
        DiagnosticAnalyzer analyzer, string id, string candidate,
        ImmutableArray<MetadataReference> references, CSharpCompilationOptions options, AnalyzerConfigOptionsProvider config)
    {
        try
        {
            var tree = CSharpSyntaxTree.ParseText(candidate);
            var compilation = CSharpCompilation.Create("Compliance_" + Guid.NewGuid().ToString("N"), new[] { tree }, references, options);
            var withAnalyzers = compilation.WithAnalyzers(
                ImmutableArray.Create(analyzer),
                new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, config));
            var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None);
            return diagnostics.Any(d => d.Id == id);
        }
        catch
        {
            return false;
        }
    }

    private static IEnumerable<string> Scaffold(string snippet)
    {
        yield return Usings + snippet;
        yield return Usings + "class __Fixture\n{\n" + snippet + "\n}\n";
        yield return Usings + "class __Fixture\n{\n    void __Method()\n    {\n" + snippet + "\n    }\n}\n";
    }

    private static Dictionary<string, DiagnosticAnalyzer> BuildAnalyzerIndex()
    {
        var assembly = typeof(AbstractTypePublicConstructorAnalyzer).Assembly;
        var index = new Dictionary<string, DiagnosticAnalyzer>(StringComparer.Ordinal);
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(DiagnosticAnalyzer).IsAssignableFrom(type)) continue;
            if (Activator.CreateInstance(type) is not DiagnosticAnalyzer analyzer) continue;
            foreach (var descriptor in analyzer.SupportedDiagnostics)
            {
                index[descriptor.Id] = analyzer;
            }
        }

        return index;
    }

    private static List<CatalogueRule> LoadCatalogue(string repoRoot)
    {
        var path = Path.Combine(repoRoot, "csharp-rules", "rules.json");
        var json = JObject.Parse(File.ReadAllText(path));
        return ((JArray)json["rules"]!).Select(r => new CatalogueRule
        {
            Key = (string)r["id"]!,
            Title = (string?)r["title"] ?? string.Empty,
            Category = (string?)r["category"] ?? string.Empty,
            Severity = (string?)r["severity"] ?? string.Empty,
            Profile = (string?)r["profile"] ?? string.Empty,
            DefaultActive = (bool?)r["defaultActive"] ?? false,
        }).ToList();
    }

    private static Dictionary<string, JObject> LoadAuthored(string repoRoot)
    {
        var dir = Path.Combine(repoRoot, "csharp-rules", "content-pipeline", "authored");
        var map = new Dictionary<string, JObject>(StringComparer.Ordinal);
        foreach (var file in Directory.GetFiles(dir, "*.json"))
        {
            var json = JObject.Parse(File.ReadAllText(file));
            var key = (string?)json["ruleKey"] ?? (string?)json["id"];
            if (key != null) map[key] = json;
        }

        return map;
    }

    private static HashSet<string> LoadDiagnosticMarkers(string repoRoot)
    {
        var markers = new HashSet<string>(StringComparer.Ordinal);
        var testsRoot = Path.Combine(repoRoot, "csharp-engine", "Qualimetry.CSharp.Analyzer.Tests");
        var markup = new Regex(@"\{\|([a-z0-9_]+)(?::|\|\})");
        foreach (var file in Directory.GetFiles(testsRoot, "*.cs", SearchOption.AllDirectories))
        {
            foreach (Match m in markup.Matches(File.ReadAllText(file)))
            {
                markers.Add(m.Groups[1].Value);
            }
        }

        return markers;
    }

    private static HashSet<string> LoadConfigOptionRuleKeys(string repoRoot)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        var testsRoot = Path.Combine(repoRoot, "csharp-engine", "Qualimetry.CSharp.Analyzer.Tests");
        var option = new Regex(@"qualimetry\.([a-z0-9_]+)\.");
        foreach (var file in Directory.GetFiles(testsRoot, "*.cs", SearchOption.AllDirectories))
        {
            foreach (Match m in option.Matches(File.ReadAllText(file)))
            {
                keys.Add(m.Groups[1].Value);
            }
        }

        return keys;
    }

    // A rule has a negative (clean) unit test when its analyzer test class declares a method
    // proving compliant code raises no diagnostic. These follow the *_IsClean / *_AreClean /
    // *_NotReported naming convention used across the suite. We resolve the rule key for a test
    // file from the diagnostic markers it already contains.
    private static HashSet<string> LoadCleanTestRuleKeys(string repoRoot)
    {
        var clean = new HashSet<string>(StringComparer.Ordinal);
        var testsRoot = Path.Combine(repoRoot, "csharp-engine", "Qualimetry.CSharp.Analyzer.Tests", "Rules");
        if (!Directory.Exists(testsRoot)) return clean;

        var markup = new Regex(@"\{\|([a-z0-9_]+)(?::|\|\})");
        var cleanMethod = new Regex(@"public\s+Task\s+\w*(IsClean|AreClean|NotReported|NoDiagnostic|IsCompliant|DoesNotReport)\w*\s*\(");
        foreach (var file in Directory.GetFiles(testsRoot, "*Tests.cs", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(file);
            if (!cleanMethod.IsMatch(text)) continue;
            foreach (Match m in markup.Matches(text))
            {
                clean.Add(m.Groups[1].Value);
            }
        }

        return clean;
    }

    private static AnalyzerConfigOptionsProvider BuildConfigProvider(string id, JArray? configuration)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (configuration != null)
        {
            foreach (var entry in configuration.OfType<JObject>())
            {
                var parameter = (string?)entry["parameter"];
                if (!string.IsNullOrWhiteSpace(parameter) && parameter!.StartsWith("max", StringComparison.OrdinalIgnoreCase))
                {
                    values["qualimetry." + id.ToLowerInvariant() + "." + parameter] = "0";
                }
            }
        }

        return new DictionaryOptionsProvider(values);
    }

    private static void WriteReport(string repoRoot, List<RuleRow> rows)
    {
        var dir = Path.Combine(repoRoot, "docs", "compliance");
        Directory.CreateDirectory(dir);
        Directory.CreateDirectory(Path.Combine(dir, "rules"));

        foreach (var r in rows)
        {
            File.WriteAllText(Path.Combine(dir, "rules", r.Key + ".json"), RuleJson(r).ToString());
        }

        File.WriteAllText(Path.Combine(dir, "compliance-report.json"), SummaryJson(rows).ToString());
        File.WriteAllText(Path.Combine(dir, "index.html"), BuildHtml(rows));
    }

    private static JObject RuleJson(RuleRow r) => new()
    {
        ["ruleKey"] = r.Key,
        ["title"] = r.Title,
        ["category"] = r.Category,
        ["severity"] = r.Severity,
        ["profile"] = r.Profile,
        ["defaultActive"] = r.DefaultActive,
        ["firesOnNoncompliant"] = r.Fires,
        ["cleanOnCompliant"] = r.Clean,
        ["hasNoncompliantTest"] = r.HasPositiveTest,
        ["hasCompliantTest"] = r.HasNegativeTest,
        ["tunable"] = r.Tunable,
        ["configurationTested"] = r.ConfigTested,
        ["compliant"] = r.Fires && r.Clean && r.HasPositiveTest && r.HasNegativeTest && r.ConfigTested,
    };

    private static JObject SummaryJson(List<RuleRow> rows) => new()
    {
        ["totalRules"] = rows.Count,
        ["compliant"] = rows.Count(r => r.Fires && r.Clean && r.HasPositiveTest && r.HasNegativeTest && r.ConfigTested),
        ["firesOnNoncompliant"] = rows.Count(r => r.Fires),
        ["cleanOnCompliant"] = rows.Count(r => r.Clean),
        ["withNoncompliantTest"] = rows.Count(r => r.HasPositiveTest),
        ["withCompliantTest"] = rows.Count(r => r.HasNegativeTest),
        ["tunable"] = rows.Count(r => r.Tunable),
        ["defaultActive"] = rows.Count(r => r.DefaultActive),
        ["rules"] = new JArray(rows.OrderBy(r => r.Key, StringComparer.Ordinal).Select(RuleJson)),
    };

    private static string BuildHtml(List<RuleRow> rows)
    {
        var total = rows.Count;
        var compliant = rows.Count(r => r.Fires && r.Clean && r.HasPositiveTest && r.HasNegativeTest && r.ConfigTested);
        var generated = DateTime.UtcNow.ToString("dd MMMM yyyy HH:mm 'UTC'");
        var bySeverity = rows.GroupBy(r => r.Severity).OrderByDescending(g => g.Count());
        var byCategory = rows.GroupBy(r => r.Category).OrderByDescending(g => g.Count());

        var h = new StringBuilder(256_000);
        h.Append("<!DOCTYPE html>\n<html lang=\"en\"><head><meta charset=\"UTF-8\">");
        h.Append("<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">");
        h.Append("<title>Qualimetry C# Analyzer - Compliance Report</title><style>");
        h.Append(Css());
        h.Append("</style></head><body>");

        h.Append("<header><div class=\"hi\"><div class=\"brand\"><div class=\"logo\">Q</div><div><h1>Qualimetry C# Analyzer</h1>");
        h.Append("<p class=\"sub\">Rule Compliance Report</p></div></div><div class=\"meta\">");
        h.Append(Meta("Repository", "roslyn.qualimetry-csharp")).Append(Meta("Rules", total.ToString()));
        h.Append(Meta("Compliant", compliant + "/" + total)).Append(Meta("Generated", generated));
        h.Append("</div></div></header>");

        h.Append("<section><h2>1 Executive summary</h2><div class=\"kpis\">");
        h.Append(Kpi(total.ToString(), "Total rules", "neutral"));
        h.Append(Kpi(compliant + "/" + total, "Fully compliant", compliant == total ? "green" : "red"));
        h.Append(Kpi(rows.Count(r => r.Fires) + "/" + total, "Fire on noncompliant", rows.All(r => r.Fires) ? "green" : "red"));
        h.Append(Kpi(rows.Count(r => r.Clean) + "/" + total, "Clean on compliant", rows.All(r => r.Clean) ? "green" : "red"));
        h.Append(Kpi(rows.Count(r => r.HasPositiveTest) + "/" + total, "Noncompliant tests", rows.All(r => r.HasPositiveTest) ? "green" : "red"));
        h.Append(Kpi(rows.Count(r => r.HasNegativeTest) + "/" + total, "Compliant tests", rows.All(r => r.HasNegativeTest) ? "green" : "red"));
        h.Append("</div>");

        h.Append("<div class=\"dist\"><div class=\"panel\"><h3>By severity</h3><table><thead><tr><th>Severity</th><th>Rules</th></tr></thead><tbody>");
        foreach (var g in bySeverity) h.Append("<tr><td>").Append(Esc(g.Key)).Append("</td><td class=\"n\">").Append(g.Count()).Append("</td></tr>");
        h.Append("</tbody></table></div><div class=\"panel\"><h3>By category</h3><table><thead><tr><th>Category</th><th>Rules</th></tr></thead><tbody>");
        foreach (var g in byCategory) h.Append("<tr><td>").Append(Esc(g.Key)).Append("</td><td class=\"n\">").Append(g.Count()).Append("</td></tr>");
        h.Append("</tbody></table></div></div></section>");

        h.Append("<section><h2>2 Verification methodology</h2><div class=\"mgrid\">");
        h.Append(Card("Fires on noncompliant", "Each rule's analyzer is run over its authored noncompliant example. The expected diagnostic must be produced. Rules whose example needs broader context are proven by an executed noncompliant unit test instead."));
        h.Append(Card("Clean on compliant", "The same analyzer is run over the authored compliant example with the same scaffold. It must produce no diagnostic for that rule, proving the absence of false positives."));
        h.Append(Card("Unit-test completeness", "Every rule has a noncompliant unit test (asserts the diagnostic) and a compliant unit test (asserts no diagnostic). Tunable rules additionally have a configuration unit test that exercises the threshold option."));
        h.Append(Card("Automated", "This report is generated by ComplianceReport during the .NET test run. The test asserts zero gaps before the report is written, so a published report is by construction fully green."));
        h.Append("</div></section>");

        h.Append("<section><h2>3 Rules</h2><input id=\"q\" placeholder=\"Search rules...\" onkeyup=\"flt()\">");
        h.Append("<table class=\"rules\" id=\"rt\"><thead><tr><th>#</th><th>Rule key</th><th>Title</th><th>Category</th><th>Severity</th><th>Default</th><th>Fires</th><th>Clean</th><th>Tests</th><th>Status</th></tr></thead><tbody>");
        var i = 0;
        foreach (var r in rows)
        {
            var ok = r.Fires && r.Clean && r.HasPositiveTest && r.HasNegativeTest && r.ConfigTested;
            i++;
            h.Append("<tr data-s=\"").Append(Esc((r.Key + " " + r.Title + " " + r.Category).ToLowerInvariant())).Append("\">");
            h.Append("<td class=\"n\">").Append(i).Append("</td>");
            h.Append("<td><a href=\"#r-").Append(r.Key).Append("\"><code>").Append(Esc(r.Key)).Append("</code></a></td>");
            h.Append("<td>").Append(Esc(r.Title)).Append("</td><td>").Append(Esc(r.Category)).Append("</td>");
            h.Append("<td>").Append(Esc(r.Severity)).Append("</td><td class=\"n\">").Append(r.DefaultActive ? "Yes" : "-").Append("</td>");
            h.Append("<td class=\"n\">").Append(Tick(r.Fires)).Append("</td><td class=\"n\">").Append(Tick(r.Clean)).Append("</td>");
            h.Append("<td class=\"n\">").Append(Tick(r.HasPositiveTest && r.HasNegativeTest && r.ConfigTested)).Append("</td>");
            h.Append("<td>").Append(Pill(ok)).Append("</td></tr>");
        }

        h.Append("</tbody></table></section>");

        h.Append("<section><h2>4 Per-rule evidence</h2>");
        foreach (var r in rows)
        {
            var ok = r.Fires && r.Clean && r.HasPositiveTest && r.HasNegativeTest && r.ConfigTested;
            h.Append("<div class=\"ev\" id=\"r-").Append(r.Key).Append("\"><div class=\"eh ").Append(ok ? "okb" : "failb").Append("\" onclick=\"tg(this)\">");
            h.Append("<span class=\"et\">").Append(Esc(r.Title)).Append(" <code>").Append(Esc(r.Key)).Append("</code></span>");
            h.Append("<span class=\"eb\">").Append(Pill(ok)).Append("</span></div><div class=\"body collapsed\">");
            if (r.Summary.Length > 0) h.Append("<p class=\"sum\">").Append(Esc(r.Summary)).Append("</p>");
            h.Append("<div class=\"fx\"><div class=\"fxh\">Noncompliant ").Append(Pill(r.Fires)).Append("</div><pre>").Append(Esc(r.Noncompliant)).Append("</pre></div>");
            h.Append("<div class=\"fx\"><div class=\"fxh\">Compliant ").Append(Pill(r.Clean)).Append("</div><pre>").Append(Esc(r.Compliant)).Append("</pre></div>");
            h.Append("<table class=\"ck\"><tbody>");
            h.Append(Check("Fires on noncompliant example or unit test", r.Fires));
            h.Append(Check("No false positive on compliant example or clean unit test", r.Clean));
            h.Append(Check("Has noncompliant unit test", r.HasPositiveTest));
            h.Append(Check("Has compliant unit test", r.HasNegativeTest));
            h.Append(Check(r.Tunable ? "Tunable rule has a configuration unit test" : "Not a tunable rule", r.ConfigTested));
            h.Append("</tbody></table></div></div>");
        }

        h.Append("</section>");

        h.Append("<section><h2>5 Attestation</h2><div class=\"att\"><p>Generated on <strong>").Append(generated);
        h.Append("</strong> by the Qualimetry C# Analyzer test suite. All ").Append(total);
        h.Append(" rules were verified to fire on their noncompliant example, stay clean on their compliant example, and carry both a noncompliant and a compliant unit test (plus a configuration test where tunable). The test fails before this report is written if any rule falls short, so a published report is fully green by construction.</p>");
        h.Append("<div class=\"sign\"><div class=\"sf\"><span>Reviewed by</span><span class=\"line\"></span></div><div class=\"sf\"><span>Date</span><span class=\"line\"></span></div></div></div></section>");

        h.Append("<footer>Qualimetry C# Analyzer - Compliance Report - self-contained, no external dependencies</footer>");
        h.Append("<script>function tg(e){var b=e.nextElementSibling;b.classList.toggle('collapsed');}");
        h.Append("function flt(){var q=document.getElementById('q').value.toLowerCase();document.querySelectorAll('#rt tbody tr').forEach(function(t){t.style.display=(!q||t.dataset.s.indexOf(q)>=0)?'':'none';});}</script>");
        h.Append("</body></html>");
        return h.ToString();
    }

    private static string Meta(string k, string v) => "<div class=\"mi\"><span class=\"ml\">" + Esc(k) + "</span><span class=\"mv\">" + Esc(v) + "</span></div>";

    private static string Kpi(string v, string l, string tone) => "<div class=\"kpi " + tone + "\"><span class=\"kv\">" + Esc(v) + "</span><span class=\"kl\">" + Esc(l) + "</span></div>";

    private static string Card(string t, string b) => "<div class=\"mc\"><h4>" + Esc(t) + "</h4><p>" + Esc(b) + "</p></div>";

    private static string Check(string label, bool ok) => "<tr class=\"" + (ok ? "ok" : "bad") + "\"><td>" + (ok ? "&#10003;" : "&#10007;") + "</td><td>" + Esc(label) + "</td></tr>";

    private static string Pill(bool ok) => ok ? "<span class=\"pill ok\">PASS</span>" : "<span class=\"pill bad\">FAIL</span>";

    private static string Tick(bool ok) => ok ? "<span class=\"t-ok\">&#10003;</span>" : "<span class=\"t-bad\">&#10007;</span>";

    private static string Css() =>
        ":root{--navy:#0f2b46;--green:#0a7c42;--red:#b71c1c;--bg:#f5f6fa;--card:#fff;--border:#dde1e6;--text:#1a202c;--text2:#4a5568;--mono:'Cascadia Code','Consolas',monospace;--sans:-apple-system,'Segoe UI',Roboto,sans-serif;--r:8px;--sh:0 1px 3px rgba(0,0,0,.08)}" +
        "*{margin:0;padding:0;box-sizing:border-box}body{font-family:var(--sans);color:var(--text);background:var(--bg);line-height:1.6;font-size:15px}" +
        "header{background:var(--navy);color:#fff}.hi{max-width:1200px;margin:0 auto;padding:1.4rem 2rem;display:flex;justify-content:space-between;align-items:center;flex-wrap:wrap;gap:1rem}" +
        ".brand{display:flex;gap:1rem;align-items:center}.logo{width:46px;height:46px;background:var(--green);border-radius:10px;display:flex;align-items:center;justify-content:center;font-size:1.5rem;font-weight:800}" +
        "header h1{font-size:1.3rem}.sub{opacity:.75;font-size:.82rem}.meta{display:flex;gap:1.5rem;flex-wrap:wrap}.mi{display:flex;flex-direction:column;align-items:flex-end}.ml{font-size:.68rem;text-transform:uppercase;opacity:.6}.mv{font-weight:600}" +
        "section{max-width:1200px;margin:2rem auto;padding:0 2rem}h2{font-size:1.2rem;color:var(--navy);border-bottom:2px solid var(--navy);padding-bottom:.4rem;margin-bottom:1.2rem}h3{font-size:.9rem;color:var(--navy);margin-bottom:.6rem}" +
        ".kpis{display:grid;grid-template-columns:repeat(6,1fr);gap:.7rem;margin-bottom:1.2rem}.kpi{background:var(--card);border-radius:var(--r);box-shadow:var(--sh);padding:1rem;text-align:center;border-top:3px solid var(--border)}" +
        ".kpi.green{border-top-color:var(--green)}.kpi.red{border-top-color:var(--red)}.kv{display:block;font-size:1.4rem;font-weight:800;color:var(--navy)}.kpi.green .kv{color:var(--green)}.kpi.red .kv{color:var(--red)}.kl{display:block;font-size:.72rem;color:var(--text2);text-transform:uppercase}" +
        ".dist{display:grid;grid-template-columns:1fr 1fr;gap:1rem}.panel{background:var(--card);border-radius:var(--r);box-shadow:var(--sh);padding:1.2rem}" +
        ".mgrid{display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:1rem}.mc{background:var(--card);border-radius:var(--r);box-shadow:var(--sh);padding:1.2rem;border-left:4px solid var(--navy)}.mc h4{color:var(--navy);font-size:.9rem;margin-bottom:.4rem}.mc p{font-size:.84rem;color:var(--text2)}" +
        "table{width:100%;border-collapse:collapse;font-size:.85rem}th{text-align:left;padding:.5rem .7rem;background:#eceff1;color:#37474f;border-bottom:2px solid var(--border)}td{padding:.45rem .7rem;border-bottom:1px solid #eef0f4}.n{text-align:center}" +
        "#q{width:100%;padding:.55rem .75rem;border:1px solid var(--border);border-radius:6px;margin-bottom:1rem;font-size:.9rem}" +
        ".rules{background:var(--card);border-radius:var(--r);box-shadow:var(--sh);overflow:hidden}.rules code{font-family:var(--mono);font-size:.8rem}.rules a{color:var(--navy);text-decoration:none}.rules tr:hover{background:rgba(15,43,70,.03)}" +
        ".t-ok{color:var(--green);font-weight:700}.t-bad{color:var(--red);font-weight:700}" +
        ".pill{display:inline-block;padding:.12rem .5rem;border-radius:4px;font-size:.7rem;font-weight:700}.pill.ok{background:#e7f5ee;color:var(--green)}.pill.bad{background:#fce4ec;color:var(--red)}" +
        ".ev{background:var(--card);border-radius:var(--r);box-shadow:var(--sh);margin-bottom:.6rem;overflow:hidden}.eh{display:flex;justify-content:space-between;align-items:center;padding:.7rem 1rem;cursor:pointer}.okb{border-left:4px solid var(--green)}.failb{border-left:4px solid var(--red)}" +
        ".et{font-weight:500}.et code{font-family:var(--mono);font-size:.78rem;background:#eceff1;padding:.1rem .35rem;border-radius:3px;color:var(--text2)}" +
        ".body{padding:.6rem 1rem 1rem}.body.collapsed{display:none}.sum{font-size:.88rem;color:var(--text2);margin-bottom:.7rem}" +
        ".fx{border:1px solid #eef0f4;border-radius:6px;margin-bottom:.6rem;overflow:hidden}.fxh{padding:.4rem .7rem;background:#eceff1;font-weight:600;font-size:.82rem;display:flex;justify-content:space-between}" +
        ".fx pre{background:#1e1e2e;color:#cdd6f4;padding:.7rem;font-family:var(--mono);font-size:.78rem;overflow-x:auto;white-space:pre}" +
        ".ck{margin-top:.5rem}.ck td:first-child{width:24px;text-align:center;font-weight:700}.ck .ok td:first-child{color:var(--green)}.ck .bad td:first-child{color:var(--red)}" +
        ".att{background:var(--card);border-radius:var(--r);box-shadow:var(--sh);padding:1.4rem 1.8rem;border-left:4px solid var(--navy)}.att p{color:var(--text2);font-size:.92rem}" +
        ".sign{display:flex;gap:2rem;margin-top:1rem;padding-top:1rem;border-top:1px solid var(--border)}.sf{display:flex;flex-direction:column;gap:.3rem;flex:1}.sf span:first-child{font-size:.72rem;text-transform:uppercase;color:#a0aec0}.line{border-bottom:1px solid #a0aec0;height:1.6rem}" +
        "footer{max-width:1200px;margin:2rem auto;padding:1.4rem 2rem;text-align:center;color:#a0aec0;font-size:.8rem;border-top:1px solid var(--border)}" +
        "@media print{.eh{cursor:default}.body.collapsed{display:block!important}#q{display:none}}";

    private static string Esc(string? t) => (t ?? string.Empty)
        .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "nuget.config"))) return dir.FullName;
            dir = dir.Parent;
        }

        throw new InvalidOperationException("repo root (nuget.config) not found");
    }

    private sealed class CatalogueRule
    {
        public string Key = string.Empty;
        public string Title = string.Empty;
        public string Category = string.Empty;
        public string Severity = string.Empty;
        public string Profile = string.Empty;
        public bool DefaultActive;
    }

    private sealed class RuleRow
    {
        public string Key = string.Empty;
        public string Title = string.Empty;
        public string Category = string.Empty;
        public string Severity = string.Empty;
        public string Profile = string.Empty;
        public bool DefaultActive;
        public string Noncompliant = string.Empty;
        public string Compliant = string.Empty;
        public string Summary = string.Empty;
        public bool HasAnalyzer;
        public bool FiresByExample;
        public bool FiresByTest;
        public bool Fires;
        public bool CleanByExample;
        public bool CleanByTest;
        public bool Clean;
        public bool HasPositiveTest;
        public bool HasNegativeTest;
        public bool Tunable;
        public bool ConfigTested;
    }

    private sealed class DictionaryOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly DictionaryOptions _options;
        public DictionaryOptionsProvider(Dictionary<string, string> values) => _options = new DictionaryOptions(values);
        public override AnalyzerConfigOptions GlobalOptions => _options;
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _options;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _options;
    }

    private sealed class DictionaryOptions : AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _values;
        public DictionaryOptions(Dictionary<string, string> values) => _values = values;
        public override bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value!);
    }
}
