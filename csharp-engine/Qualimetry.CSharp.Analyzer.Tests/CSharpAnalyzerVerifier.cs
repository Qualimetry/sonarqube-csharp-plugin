using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Qualimetry.CSharp.Analyzer.Tests;

internal static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static Task VerifyAsync(string source, params (string key, string value)[] options)
    {
        var test = new Test { TestCode = source };
        foreach (var (key, value) in options)
        {
            test.TestState.AnalyzerConfigFiles.Add(
                ("/.globalconfig", $"is_global = true\n{key} = {value}\n"));
        }

        return test.RunAsync(CancellationToken.None);
    }

    public static Task VerifyFilesAsync(params (string path, string source)[] files)
    {
        var test = new Test();
        foreach (var (path, source) in files)
        {
            test.TestState.Sources.Add((path, source));
        }

        return test.RunAsync(CancellationToken.None);
    }

    public static Task VerifyFilesWithOptionsAsync((string key, string value)[] options, params (string path, string source)[] files)
    {
        var test = new Test();
        foreach (var (path, source) in files)
        {
            test.TestState.Sources.Add((path, source));
        }

        if (options.Length > 0)
        {
            var builder = new System.Text.StringBuilder("is_global = true\n");
            foreach (var (key, value) in options)
            {
                builder.Append(key).Append(" = ").Append(value).Append('\n');
            }

            test.TestState.AnalyzerConfigFiles.Add(("/.globalconfig", builder.ToString()));
        }

        return test.RunAsync(CancellationToken.None);
    }

    public static Task VerifyWithSonarLintAsync(string source, string sonarLintXml)
    {
        var test = new Test { TestCode = source };
        test.TestState.AdditionalFiles.Add(("SonarLint.xml", sonarLintXml));
        return test.RunAsync(CancellationToken.None);
    }

    private sealed class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
            CompilerDiagnostics = CompilerDiagnostics.None;
        }
    }
}
