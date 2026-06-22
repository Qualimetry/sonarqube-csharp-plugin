package com.qualimetry.csharp;

import org.sonar.api.Plugin;

public final class QualimetryCSharpPlugin implements Plugin {

    @Override
    public void define(Context context) {
        Catalogue catalogue = Catalogue.load();
        context.addExtension(new QualimetryRulesDefinition(catalogue));
        context.addExtension(new QualimetryProfiles(catalogue));
        context.addExtensions(QualimetryProvisioning.definitions());
    }
}
