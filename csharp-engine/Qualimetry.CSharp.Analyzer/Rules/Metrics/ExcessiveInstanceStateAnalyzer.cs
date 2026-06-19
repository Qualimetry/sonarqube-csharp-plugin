using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExcessiveInstanceStateAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxInstanceFields = 15;
    private const string Option = "qualimetry.qa_metrics_excessive_instance_state.maxinstancefields";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0099);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;
        var threshold = OptionReader.ReadInt(context, declaration.SyntaxTree, Option, DefaultMaxInstanceFields);

        var slots = 0;
        foreach (var member in declaration.Members)
        {
            switch (member)
            {
                case FieldDeclarationSyntax field
                    when !field.Modifiers.Any(SyntaxKind.StaticKeyword)
                        && !field.Modifiers.Any(SyntaxKind.ConstKeyword):
                    slots += field.Declaration.Variables.Count;
                    break;
                case PropertyDeclarationSyntax property
                    when !property.Modifiers.Any(SyntaxKind.StaticKeyword) && IsAutoProperty(property):
                    slots++;
                    break;
            }
        }

        if (slots > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0099,
                declaration.Identifier.GetLocation(),
                slots.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }

    private static bool IsAutoProperty(PropertyDeclarationSyntax property)
    {
        if (property.ExpressionBody != null || property.AccessorList == null)
        {
            return false;
        }

        return property.AccessorList.Accessors.All(a => a.Body == null && a.ExpressionBody == null);
    }
}
