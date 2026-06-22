using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UriMemberAsStringAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0169);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeField, SyntaxKind.FieldDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;

        if (!IsString(context, property.Type) || !HasUriName(property.Identifier.ValueText))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0169, property.Identifier.GetLocation()));
    }

    private static void AnalyzeField(SyntaxNodeAnalysisContext context)
    {
        var field = (FieldDeclarationSyntax)context.Node;

        if (!IsString(context, field.Declaration.Type))
        {
            return;
        }

        foreach (var variable in field.Declaration.Variables)
        {
            if (HasUriName(variable.Identifier.ValueText))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0169, variable.Identifier.GetLocation()));
            }
        }
    }

    private static bool IsString(SyntaxNodeAnalysisContext context, TypeSyntax typeSyntax)
    {
        return context.SemanticModel.GetTypeInfo(typeSyntax, context.CancellationToken).Type?.SpecialType == SpecialType.System_String;
    }

    private static bool HasUriName(string name)
    {
        return name.EndsWith("Uri", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("Url", StringComparison.OrdinalIgnoreCase);
    }
}
