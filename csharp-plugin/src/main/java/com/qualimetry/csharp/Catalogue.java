package com.qualimetry.csharp;

import com.google.gson.Gson;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.UncheckedIOException;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.List;

public final class Catalogue {

    private static final String RULES_RESOURCE = "/com/qualimetry/csharp/rules.json";
    private static final String HTML_RESOURCE_PREFIX = "/com/qualimetry/csharp/html/";

    private final List<RuleEntry> rules;

    private Catalogue(List<RuleEntry> rules) {
        this.rules = rules;
    }

    public static Catalogue load() {
        try (InputStream stream = Catalogue.class.getResourceAsStream(RULES_RESOURCE)) {
            if (stream == null) {
                throw new IllegalStateException("Missing catalogue resource " + RULES_RESOURCE);
            }
            CatalogueModel model = new Gson().fromJson(
                new InputStreamReader(stream, StandardCharsets.UTF_8), CatalogueModel.class);
            if (model == null || model.rules == null) {
                throw new IllegalStateException("Catalogue resource is empty: " + RULES_RESOURCE);
            }
            return new Catalogue(model.rules);
        } catch (IOException e) {
            throw new UncheckedIOException(e);
        }
    }

    public List<RuleEntry> rules() {
        return rules;
    }

    public String htmlDescription(String id) {
        try (InputStream stream = Catalogue.class.getResourceAsStream(HTML_RESOURCE_PREFIX + id + ".html")) {
            if (stream == null) {
                return "<p>" + id + "</p>";
            }
            return new String(stream.readAllBytes(), StandardCharsets.UTF_8);
        } catch (IOException e) {
            throw new UncheckedIOException(e);
        }
    }

    private static final class CatalogueModel {
        List<RuleEntry> rules;
    }

    public static final class RuleEntry {
        public String id;
        public String key;
        public String title;
        public String severity;
        public String type;
        public String category;
        public List<String> tags;
        public boolean defaultActive;
        public String helpUrl;
        public List<RuleParam> parameters;
    }

    public static final class RuleParam {
        public String key;
        public String name;
        public String type;
        public String defaultValue;
        public String description;
    }
}
