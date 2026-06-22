package com.qualimetry.csharp;

import static org.assertj.core.api.Assertions.assertThat;

import com.google.gson.Gson;
import com.google.gson.JsonObject;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
import org.junit.jupiter.api.Test;
import org.sonar.api.config.PropertyDefinition;

class QualimetryProvisioningTest {

    private static final String JSON_RESOURCE = "/com/qualimetry/csharp/RoslynRules.qualimetry-csharp.json";

    @Test
    void exports_the_three_settings_the_scanner_requires_to_fetch_the_payload() {
        Map<String, String> exported = QualimetryProvisioning.definitions().stream()
            .collect(Collectors.toMap(PropertyDefinition::key, PropertyDefinition::defaultValue));

        assertThat(exported).containsEntry(QualimetryProvisioning.KEY_PLUGIN_KEY, QualimetryCSharp.PLUGIN_KEY);
        assertThat(exported).containsEntry(QualimetryProvisioning.KEY_PLUGIN_VERSION, QualimetryCSharp.PLUGIN_VERSION);
        assertThat(exported)
            .containsEntry(QualimetryProvisioning.KEY_STATIC_RESOURCE_NAME, QualimetryCSharp.STATIC_RESOURCE_NAME);
        assertThat(exported).containsEntry(QualimetryProvisioning.KEY_NUGET_PACKAGE_ID, QualimetryCSharp.NUGET_PACKAGE_ID);

        assertThat(exported.keySet())
            .allSatisfy(key -> assertThat(key).startsWith(QualimetryProvisioning.PROPERTY_PREFIX + "."));
        assertThat(exported.values()).noneMatch(value -> value == null || value.isBlank());
    }

    @Test
    void embedded_static_payload_contains_the_analyzer_assemblies() throws Exception {
        Set<String> entries = new HashSet<>();
        try (InputStream stream = getClass().getResourceAsStream("/" + QualimetryProvisioning.STATIC_RESOURCE_PATH);
            ZipInputStream zip = new ZipInputStream(stream)) {
            ZipEntry entry;
            while ((entry = zip.getNextEntry()) != null) {
                entries.add(entry.getName());
            }
        }

        assertThat(entries).contains("analyzers/dotnet/cs/Qualimetry.CSharp.Analyzer.dll");
        assertThat(entries).contains("analyzers/dotnet/cs/Qualimetry.CSharp.Analyzer.CodeFixes.dll");
        assertThat(entries).contains("config/analyzers.globalconfig");
    }

    @Test
    void provisioning_manifest_matches_the_catalogue_and_exports() {
        JsonObject manifest = loadManifest();

        assertThat(manifest.get("pluginKey").getAsString()).isEqualTo(QualimetryCSharp.PLUGIN_KEY);
        assertThat(manifest.get("pluginVersion").getAsString()).isEqualTo(QualimetryCSharp.PLUGIN_VERSION);
        assertThat(manifest.get("repositoryKey").getAsString()).isEqualTo(QualimetryCSharp.REPOSITORY_KEY);
        assertThat(manifest.get("staticResourceName").getAsString()).isEqualTo(QualimetryCSharp.STATIC_RESOURCE_NAME);
        assertThat(manifest.get("nugetPackageId").getAsString()).isEqualTo(QualimetryCSharp.NUGET_PACKAGE_ID);

        Catalogue catalogue = Catalogue.load();
        Set<String> catalogueActive = catalogue.rules().stream()
            .filter(r -> r.defaultActive)
            .map(r -> r.id)
            .collect(Collectors.toSet());
        Set<String> catalogueKeys = catalogue.rules().stream().map(r -> r.id).collect(Collectors.toSet());

        Set<String> manifestKeys = new HashSet<>();
        Set<String> manifestActive = new HashSet<>();
        manifest.getAsJsonArray("rules").forEach(element -> {
            JsonObject rule = element.getAsJsonObject();
            String key = rule.get("ruleKey").getAsString();
            manifestKeys.add(key);
            if (rule.get("defaultActive").getAsBoolean()) {
                manifestActive.add(key);
            }
        });

        assertThat(manifestKeys).isEqualTo(catalogueKeys);
        assertThat(manifestActive).isEqualTo(catalogueActive);
    }

    private JsonObject loadManifest() {
        try (InputStream stream = getClass().getResourceAsStream(JSON_RESOURCE)) {
            assertThat(stream).as("provisioning manifest resource").isNotNull();
            return new Gson().fromJson(new InputStreamReader(stream, StandardCharsets.UTF_8), JsonObject.class);
        } catch (Exception e) {
            throw new IllegalStateException(e);
        }
    }

    @Test
    void plugin_registers_provisioning_property_definitions() {
        List<PropertyDefinition> definitions = QualimetryProvisioning.definitions();
        assertThat(definitions).hasSize(7);
        assertThat(definitions).allSatisfy(definition -> assertThat(definition.key()).isNotBlank());
    }
}
