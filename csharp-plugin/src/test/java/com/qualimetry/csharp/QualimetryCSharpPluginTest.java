package com.qualimetry.csharp;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.mock;

import org.junit.jupiter.api.Test;
import org.sonar.api.Plugin;
import org.sonar.api.SonarRuntime;
import org.sonar.api.config.PropertyDefinition;
import org.sonar.api.server.profile.BuiltInQualityProfilesDefinition;
import org.sonar.api.server.rule.RulesDefinition;

class QualimetryCSharpPluginTest {

    private Plugin.Context createContext() {
        return new Plugin.Context(mock(SonarRuntime.class));
    }

    @Test
    void definesEveryExtensionWithoutMissingApi() {
        Plugin.Context context = createContext();
        new QualimetryCSharpPlugin().define(context);
        assertThat(context.getExtensions()).hasSize(9);
    }

    @Test
    void registersRulesAndProfilesAndProvisioning() {
        Plugin.Context context = createContext();
        new QualimetryCSharpPlugin().define(context);

        assertThat(context.getExtensions())
            .anyMatch(e -> e instanceof RulesDefinition)
            .anyMatch(e -> e instanceof BuiltInQualityProfilesDefinition);
        assertThat(context.getExtensions().stream().filter(e -> e instanceof PropertyDefinition).count())
            .isEqualTo(7);
    }
}
