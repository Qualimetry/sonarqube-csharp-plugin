package com.qualimetry.csharp;

import static org.assertj.core.api.Assertions.assertThat;

import java.util.HashSet;
import java.util.List;
import java.util.Set;
import org.junit.jupiter.api.Test;
import org.sonar.api.config.PropertyDefinition;

/**
 * Reproduces the SonarQube startup crash class: the platform's bundled C# analyzer registers a
 * {@code PropertyDefinition} for every {@code sonaranalyzer-cs.*} key, and SonarQube wires each one
 * into a Spring container with bean overriding disabled. Any plugin that also registers one of those
 * keys triggers {@code BeanDefinitionOverrideException} and the server hard-stops. v1.0.3 did exactly
 * that. Reverting the fix (prefix back to {@code sonaranalyzer-cs}) re-collides and fails these tests.
 */
class RoslynAnalyzerProviderCollisionTest {

    private static final String BUNDLED_ANALYZER_NAMESPACE = "sonaranalyzer-" + QualimetryCSharp.LANGUAGE_KEY;

    private static final Set<String> RESERVED_BUNDLED_KEYS = Set.of(
        BUNDLED_ANALYZER_NAMESPACE + ".pluginKey",
        BUNDLED_ANALYZER_NAMESPACE + ".pluginVersion",
        BUNDLED_ANALYZER_NAMESPACE + ".staticResourceName",
        BUNDLED_ANALYZER_NAMESPACE + ".analyzerId",
        BUNDLED_ANALYZER_NAMESPACE + ".ruleNamespace",
        BUNDLED_ANALYZER_NAMESPACE + ".nuget.packageId",
        BUNDLED_ANALYZER_NAMESPACE + ".nuget.packageVersion");

    @Test
    void registering_our_settings_after_the_bundled_analyzer_does_not_override_a_bean() {
        // Models SonarQube's bean factory (overriding disabled): bean name == PropertyDefinition key.
        Set<String> beanNames = new HashSet<>(RESERVED_BUNDLED_KEYS);
        for (PropertyDefinition definition : QualimetryProvisioning.definitions()) {
            boolean fresh = beanNames.add(definition.key());
            assertThat(fresh)
                .as("setting '%s' collides with the bundled C# analyzer and would crash SonarQube on load",
                    definition.key())
                .isTrue();
        }
    }

    @Test
    void no_setting_lives_in_the_reserved_bundled_analyzer_namespace() {
        assertThat(QualimetryProvisioning.definitions())
            .extracting(PropertyDefinition::key)
            .noneMatch(key -> key.startsWith(BUNDLED_ANALYZER_NAMESPACE + "."));
    }

    @Test
    void rules_repository_uses_the_roslyn_provisioning_namespace_so_the_scanner_imports_them() {
        // SonarScanner for .NET only provisions analyzers whose repo key starts with "roslyn.",
        // and reads settings prefixed with the repo key minus that prefix.
        assertThat(QualimetryCSharp.REPOSITORY_KEY).startsWith("roslyn.");
        assertThat(QualimetryCSharp.REPOSITORY_KEY)
            .isEqualTo("roslyn." + QualimetryProvisioning.PROPERTY_PREFIX);

        List<PropertyDefinition> definitions = QualimetryProvisioning.definitions();
        assertThat(definitions)
            .extracting(PropertyDefinition::key)
            .allMatch(key -> key.startsWith(QualimetryProvisioning.PROPERTY_PREFIX + "."));
    }

    @Test
    void plugin_key_setting_matches_the_sanitized_manifest_key_used_for_static_resources() {
        // sonar-packaging strips non-alphanumerics, so the served static path is /static/qualimetrycsharp/.
        assertThat(QualimetryCSharp.PLUGIN_KEY).isEqualTo("qualimetrycsharp");
        assertThat(QualimetryProvisioning.definitions())
            .filteredOn(d -> d.key().equals(QualimetryProvisioning.KEY_PLUGIN_KEY))
            .singleElement()
            .extracting(PropertyDefinition::defaultValue)
            .isEqualTo(QualimetryCSharp.PLUGIN_KEY);
    }
}
