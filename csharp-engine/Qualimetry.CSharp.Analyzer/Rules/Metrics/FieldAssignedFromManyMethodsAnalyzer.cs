using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FieldAssignedFromManyMethodsAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxAssigningMethods = 3;
    private const string Option = "qualimetry.qa_metrics_field_assigned_from_many_methods.maxassigningmethods";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0068);

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
        var threshold = OptionReader.ReadInt(context, declaration.SyntaxTree, Option, DefaultMaxAssigningMethods);

        var fieldIdentifiers = new Dictionary<string, SyntaxToken>(System.StringComparer.Ordinal);
        foreach (var member in declaration.Members)
        {
            if (member is FieldDeclarationSyntax field && !field.Modifiers.Any(SyntaxKind.ConstKeyword))
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    var name = variable.Identifier.ValueText;
                    if (!fieldIdentifiers.ContainsKey(name))
                    {
                        fieldIdentifiers[name] = variable.Identifier;
                    }
                }
            }
        }

        if (fieldIdentifiers.Count == 0)
        {
            return;
        }

        var assigningMethods = new Dictionary<string, HashSet<MethodDeclarationSyntax>>(System.StringComparer.Ordinal);
        foreach (var assignment in declaration.DescendantNodes())
        {
            if (assignment is not AssignmentExpressionSyntax expression)
            {
                continue;
            }

            var name = ResolveTargetFieldName(expression.Left);
            if (name == null || !fieldIdentifiers.ContainsKey(name))
            {
                continue;
            }

            var method = expression.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            if (method == null)
            {
                continue;
            }

            if (!assigningMethods.TryGetValue(name, out var methods))
            {
                methods = new HashSet<MethodDeclarationSyntax>();
                assigningMethods[name] = methods;
            }

            methods.Add(method);
        }

        foreach (var pair in assigningMethods)
        {
            if (pair.Value.Count > threshold)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptors.QCS0068,
                    fieldIdentifiers[pair.Key].GetLocation(),
                    pair.Key,
                    pair.Value.Count.ToString(CultureInfo.InvariantCulture),
                    threshold.ToString(CultureInfo.InvariantCulture)));
            }
        }
    }

    private static string? ResolveTargetFieldName(ExpressionSyntax left)
    {
        switch (left)
        {
            case IdentifierNameSyntax identifier:
                return identifier.Identifier.ValueText;
            case MemberAccessExpressionSyntax member when member.Expression is ThisExpressionSyntax:
                return member.Name.Identifier.ValueText;
            default:
                return null;
        }
    }
}
