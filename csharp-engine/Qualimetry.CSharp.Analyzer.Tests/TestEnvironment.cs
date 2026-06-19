using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Qualimetry.CSharp.Analyzer.Tests;

internal static class TestEnvironment
{
    [ModuleInitializer]
    public static void Initialize()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "nuget.config")))
            {
                Directory.SetCurrentDirectory(directory.FullName);
                return;
            }

            directory = directory.Parent;
        }
    }
}
