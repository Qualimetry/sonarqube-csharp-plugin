using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MutableRecordPropertyAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0138);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var record = (TypeDeclarationSyntax)context.Node;

        foreach (var member in record.Members)
        {
            if (member is not PropertyDeclarationSyntax property
                || property.Modifiers.Any(SyntaxKind.StaticKeyword)
                || property.AccessorList is null)
            {
                continue;
            }

            foreach (var accessor in property.AccessorList.Accessors)
            {
                if (accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0138, accessor.Keyword.GetLocation()));
                }
            }
        }
    }
}
