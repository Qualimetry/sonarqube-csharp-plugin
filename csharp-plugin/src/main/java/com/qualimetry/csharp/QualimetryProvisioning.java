package com.qualimetry.csharp;

import java.util.List;
import org.sonar.api.config.PropertyDefinition;

/**
 * Exports the server settings that SonarScanner for .NET reads to download and inject the embedded
 * Qualimetry Roslyn analyzer during an MSBuild scan. The scanner provisions any analyzer whose rules
 * live in a {@code roslyn.<partial>} repository and reads its {@code <partial>.pluginKey},
 * {@code <partial>.pluginVersion} and {@code <partial>.staticResourceName} settings to fetch
 * {@code static/<pluginKey>/<staticResourceName>} from SonarQube. The prefix is the plugin's own
 * partial repository key, never the bundled analyzer's reserved {@code sonaranalyzer-cs} namespace.
 */
public final class QualimetryProvisioning {

    public static final String PROPERTY_PREFIX = QualimetryCSharp.ROSLYN_PARTIAL_KEY;

    public static final String KEY_ANALYZER_ID = PROPERTY_PREFIX + ".analyzerId";
    public static final String KEY_RULE_NAMESPACE = PROPERTY_PREFIX + ".ruleNamespace";
    public static final String KEY_PLUGIN_KEY = PROPERTY_PREFIX + ".pluginKey";
    public static final String KEY_PLUGIN_VERSION = PROPERTY_PREFIX + ".pluginVersion";
    public static final String KEY_STATIC_RESOURCE_NAME = PROPERTY_PREFIX + ".staticResourceName";
    public static final String KEY_NUGET_PACKAGE_ID = PROPERTY_PREFIX + ".nuget.packageId";
    public static final String KEY_NUGET_PACKAGE_VERSION = PROPERTY_PREFIX + ".nuget.packageVersion";

    public static final String STATIC_RESOURCE_PATH = "static/" + QualimetryCSharp.STATIC_RESOURCE_NAME;

    private QualimetryProvisioning() {
    }

    public static List<PropertyDefinition> definitions() {
        return List.of(
            export(KEY_PLUGIN_KEY, QualimetryCSharp.PLUGIN_KEY),
            export(KEY_PLUGIN_VERSION, QualimetryCSharp.PLUGIN_VERSION),
            export(KEY_STATIC_RESOURCE_NAME, QualimetryCSharp.STATIC_RESOURCE_NAME),
            export(KEY_ANALYZER_ID, QualimetryCSharp.ANALYZER_ID),
            export(KEY_RULE_NAMESPACE, QualimetryCSharp.ANALYZER_ID),
            export(KEY_NUGET_PACKAGE_ID, QualimetryCSharp.NUGET_PACKAGE_ID),
            export(KEY_NUGET_PACKAGE_VERSION, QualimetryCSharp.PLUGIN_VERSION));
    }

    private static PropertyDefinition export(String key, String value) {
        return PropertyDefinition.builder(key)
            .name(key)
            .defaultValue(value)
            .hidden()
            .build();
    }
}
