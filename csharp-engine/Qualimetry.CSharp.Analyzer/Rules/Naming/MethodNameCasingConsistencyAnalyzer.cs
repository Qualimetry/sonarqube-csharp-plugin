using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Naming;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MethodNameCasingConsistencyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0036);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(
            Analyze,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.RecordStructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (TypeDeclarationSyntax)context.Node;
        var established = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var member in declaration.Members)
        {
            if (member is not MethodDeclarationSyntax method)
            {
                continue;
            }

            var identifier = method.Identifier;
            var name = identifier.ValueText;

            if (!established.TryGetValue(name, out var firstSpelling))
            {
                established.Add(name, name);
                continue;
            }

            if (!string.Equals(firstSpelling, name, StringComparison.Ordinal))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0036, identifier.GetLocation(), name));
            }
        }
    }
}
