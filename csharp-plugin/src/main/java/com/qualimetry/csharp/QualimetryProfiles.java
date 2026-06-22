package com.qualimetry.csharp;

import org.sonar.api.server.profile.BuiltInQualityProfilesDefinition;

public final class QualimetryProfiles implements BuiltInQualityProfilesDefinition {

    private final Catalogue catalogue;

    public QualimetryProfiles(Catalogue catalogue) {
        this.catalogue = catalogue;
    }

    @Override
    public void define(Context context) {
        defineRecommended(context);
        defineAll(context);
    }

    private void defineRecommended(Context context) {
        NewBuiltInQualityProfile profile = context
            .createBuiltInQualityProfile(QualimetryCSharp.PROFILE_RECOMMENDED, QualimetryCSharp.LANGUAGE_KEY);
        profile.setDefault(false);
        for (Catalogue.RuleEntry entry : catalogue.rules()) {
            if (entry.defaultActive) {
                profile.activateRule(QualimetryCSharp.REPOSITORY_KEY, entry.id);
            }
        }
        profile.done();
    }

    private void defineAll(Context context) {
        NewBuiltInQualityProfile profile = context
            .createBuiltInQualityProfile(QualimetryCSharp.PROFILE_ALL, QualimetryCSharp.LANGUAGE_KEY);
        profile.setDefault(false);
        for (Catalogue.RuleEntry entry : catalogue.rules()) {
            profile.activateRule(QualimetryCSharp.REPOSITORY_KEY, entry.id);
        }
        profile.done();
    }
}
