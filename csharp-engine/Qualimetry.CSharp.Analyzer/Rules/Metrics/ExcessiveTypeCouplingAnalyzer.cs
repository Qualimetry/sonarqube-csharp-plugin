using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Metrics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExcessiveTypeCouplingAnalyzer : DiagnosticAnalyzer
{
    private const int DefaultMaxCoupling = 20;
    private const string Option = "qualimetry.qa_metrics_excessive_type_coupling.maxcoupling";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0013);

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
        var threshold = OptionReader.ReadInt(context, declaration.SyntaxTree, Option, DefaultMaxCoupling);

        var referenced = new HashSet<string>(System.StringComparer.Ordinal);
        var ownName = declaration.Identifier.ValueText;

        foreach (var type in CollectSignatureTypes(declaration))
        {
            foreach (var name in ExtractTypeNames(type))
            {
                if (!string.Equals(name, ownName, System.StringComparison.Ordinal))
                {
                    referenced.Add(name);
                }
            }
        }

        if (referenced.Count > threshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptors.QCS0013,
                declaration.Identifier.GetLocation(),
                referenced.Count.ToString(CultureInfo.InvariantCulture),
                threshold.ToString(CultureInfo.InvariantCulture)));
        }
    }

    private static IEnumerable<TypeSyntax> CollectSignatureTypes(TypeDeclarationSyntax declaration)
    {
        foreach (var member in declaration.Members)
        {
            switch (member)
            {
                case FieldDeclarationSyntax field:
                    yield return field.Declaration.Type;
                    break;
                case PropertyDeclarationSyntax property:
                    yield return property.Type;
                    break;
                case MethodDeclarationSyntax method:
                    yield return method.ReturnType;
                    foreach (var p in method.ParameterList.Parameters)
                    {
                        if (p.Type != null)
                        {
                            yield return p.Type;
                        }
                    }

                    break;
                case ConstructorDeclarationSyntax ctor:
                    foreach (var p in ctor.ParameterList.Parameters)
                    {
                        if (p.Type != null)
                        {
                            yield return p.Type;
                        }
                    }

                    break;
            }
        }
    }

    private static IEnumerable<string> ExtractTypeNames(TypeSyntax type)
    {
        return type.DescendantNodesAndSelf()
            .OfType<SimpleNameSyntax>()
            .Select(n => n.Identifier.ValueText)
            .Where(n => !string.IsNullOrEmpty(n));
    }
}
