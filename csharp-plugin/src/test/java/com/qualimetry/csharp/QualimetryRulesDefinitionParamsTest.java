package com.qualimetry.csharp;

import static org.assertj.core.api.Assertions.assertThat;

import org.junit.jupiter.api.Test;
import org.sonar.api.server.rule.RulesDefinition;

class QualimetryRulesDefinitionParamsTest {

    @Test
    void configurableRulesDeclareNativeParametersMatchingTheCatalogue() {
        Catalogue catalogue = Catalogue.load();
        RulesDefinition.Context context = new RulesDefinition.Context();
        new QualimetryRulesDefinition(catalogue).define(context);

        RulesDefinition.Repository repository = context.repository(QualimetryCSharp.REPOSITORY_KEY);
        assertThat(repository).isNotNull();

        long configurable = catalogue.rules().stream()
            .filter(r -> r.parameters != null && !r.parameters.isEmpty())
            .count();
        assertThat(configurable).isEqualTo(17);

        for (Catalogue.RuleEntry entry : catalogue.rules()) {
            if (entry.parameters == null || entry.parameters.isEmpty()) {
                continue;
            }

            RulesDefinition.Rule rule = repository.rule(entry.id);
            assertThat(rule).as("rule %s should be registered", entry.id).isNotNull();

            for (Catalogue.RuleParam param : entry.parameters) {
                RulesDefinition.Param declared = rule.param(param.key);
                assertThat(declared)
                    .as("rule %s should declare native param '%s'", entry.id, param.key)
                    .isNotNull();
                assertThat(declared.type().toString()).isEqualTo(param.type);
                assertThat(declared.defaultValue()).isEqualTo(param.defaultValue);
                assertThat(declared.name()).isEqualTo(param.name);
                assertThat(declared.description()).isEqualTo(param.description);
            }
        }
    }
}
