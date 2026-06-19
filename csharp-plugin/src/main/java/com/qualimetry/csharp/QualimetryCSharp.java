package com.qualimetry.csharp;

public final class QualimetryCSharp {

    public static final String LANGUAGE_KEY = "cs";

    // SonarScanner for .NET only provisions analyzers whose rules live in a repository keyed
    // "roslyn.<partial>", and reads the matching "<partial>.*" server settings. The bare
    // "sonaranalyzer-cs.*" namespace belongs to the platform's bundled C# analyzer and must
    // never be redefined here (RoslynAnalyzerProviderCollisionTest guards this).
    public static final String ROSLYN_PARTIAL_KEY = "qualimetry-csharp";
    public static final String REPOSITORY_KEY = "roslyn." + ROSLYN_PARTIAL_KEY;
    public static final String REPOSITORY_NAME = "Qualimetry C#";

    public static final String PROFILE_RECOMMENDED = "Qualimetry C#";
    public static final String PROFILE_ALL = "Qualimetry Way";

    // Effective SonarQube plugin key: sonar-packaging strips non-alphanumerics from the
    // configured "qualimetry-csharp", so the manifest Plugin-Key and the static-resource path
    // are "qualimetrycsharp". The provisioning pluginKey value must match it for the scanner
    // to fetch static/<pluginKey>/<staticResourceName>.
    public static final String PLUGIN_KEY = "qualimetrycsharp";
    public static final String PLUGIN_VERSION = "1.0.7";
    public static final String ANALYZER_ID = "Qualimetry.CSharp.Analyzer";
    public static final String NUGET_PACKAGE_ID = "Qualimetry.CSharp.Analyzer";
    public static final String STATIC_RESOURCE_NAME = "Qualimetry.CSharp.Analyzer.zip";

    public static final String ROSLYN_EXPORTER_KEY = "roslyn-cs";

    private QualimetryCSharp() {
    }
}
