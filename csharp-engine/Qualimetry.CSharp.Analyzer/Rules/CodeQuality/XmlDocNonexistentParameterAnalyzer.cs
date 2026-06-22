using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class XmlDocNonexistentParameterAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0185);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        var documentation = method.GetLeadingTrivia()
            .Select(trivia => trivia.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .FirstOrDefault();
        if (documentation is null)
        {
            return;
        }

        var parameterNames = new HashSet<string>(
            method.ParameterList.Parameters.Select(p => p.Identifier.ValueText),
            StringComparer.Ordinal);

        foreach (var content in documentation.Content)
        {
            var attribute = GetParamNameAttribute(content);
            if (attribute is null)
            {
                continue;
            }

            var documentedName = attribute.Identifier.Identifier.ValueText;
            if (!parameterNames.Contains(documentedName))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0185, attribute.Identifier.GetLocation()));
            }
        }
    }

    private static XmlNameAttributeSyntax? GetParamNameAttribute(XmlNodeSyntax node)
    {
        switch (node)
        {
            case XmlElementSyntax element when element.StartTag.Name.LocalName.ValueText == "param":
                return element.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault();
            case XmlEmptyElementSyntax empty when empty.Name.LocalName.ValueText == "param":
                return empty.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault();
            default:
                return null;
        }
    }
}
