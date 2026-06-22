package com.qualimetry.csharp;

import org.sonar.api.rule.RuleStatus;
import org.sonar.api.rule.Severity;
import org.sonar.api.rules.RuleType;
import org.sonar.api.server.rule.RuleParamType;
import org.sonar.api.server.rule.RulesDefinition;

public final class QualimetryRulesDefinition implements RulesDefinition {

    private final Catalogue catalogue;

    public QualimetryRulesDefinition(Catalogue catalogue) {
        this.catalogue = catalogue;
    }

    @Override
    public void define(Context context) {
        NewRepository repository = context
            .createRepository(QualimetryCSharp.REPOSITORY_KEY, QualimetryCSharp.LANGUAGE_KEY)
            .setName(QualimetryCSharp.REPOSITORY_NAME);

        for (Catalogue.RuleEntry entry : catalogue.rules()) {
            NewRule rule = repository.createRule(entry.id)
                .setName(entry.title)
                .setHtmlDescription(catalogue.htmlDescription(entry.id))
                .setType(toType(entry.type))
                .setSeverity(toSeverity(entry.severity))
                .setStatus(RuleStatus.READY)
                .setActivatedByDefault(entry.defaultActive);

            if (entry.tags != null && !entry.tags.isEmpty()) {
                rule.setTags(entry.tags.toArray(new String[0]));
            }

            if (entry.parameters != null) {
                for (Catalogue.RuleParam param : entry.parameters) {
                    rule.createParam(param.key)
                        .setName(param.name != null ? param.name : param.key)
                        .setType(toParamType(param.type))
                        .setDefaultValue(param.defaultValue)
                        .setDescription(param.description);
                }
            }
        }

        repository.done();
    }

    private static RuleParamType toParamType(String type) {
        if (type == null) {
            return RuleParamType.STRING;
        }
        switch (type.trim().toUpperCase()) {
            case "INTEGER":
                return RuleParamType.INTEGER;
            case "BOOLEAN":
                return RuleParamType.BOOLEAN;
            case "TEXT":
                return RuleParamType.TEXT;
            default:
                return RuleParamType.STRING;
        }
    }

    private static RuleType toType(String type) {
        if (type == null) {
            return RuleType.CODE_SMELL;
        }
        switch (type.trim().toLowerCase()) {
            case "bug":
                return RuleType.BUG;
            case "vulnerability":
                return RuleType.VULNERABILITY;
            case "security hotspot":
                return RuleType.SECURITY_HOTSPOT;
            default:
                return RuleType.CODE_SMELL;
        }
    }

    private static String toSeverity(String severity) {
        if (severity == null) {
            return Severity.MAJOR;
        }
        switch (severity.trim().toUpperCase()) {
            case "BLOCKER":
                return Severity.BLOCKER;
            case "CRITICAL":
                return Severity.CRITICAL;
            case "MINOR":
                return Severity.MINOR;
            case "INFO":
                return Severity.INFO;
            default:
                return Severity.MAJOR;
        }
    }
}
