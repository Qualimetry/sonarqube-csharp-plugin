using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StatelessTypeShouldBeStaticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0002);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (declaration.Modifiers.Any(SyntaxKind.StaticKeyword)
            || declaration.Modifiers.Any(SyntaxKind.AbstractKeyword)
            || declaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return;
        }

        if (declaration.BaseList is not null)
        {
            return;
        }

        if (!declaration.Members.Any())
        {
            return;
        }

        var sawStaticMember = false;

        foreach (var member in declaration.Members)
        {
            switch (member)
            {
                case ConstructorDeclarationSyntax constructor:
                    if (!constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        if (constructor.ParameterList.Parameters.Count > 0)
                        {
                            return;
                        }
                    }

                    break;
                case BaseFieldDeclarationSyntax field:
                    if (field.Modifiers.Any(SyntaxKind.ConstKeyword))
                    {
                        break;
                    }

                    if (!field.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        return;
                    }

                    sawStaticMember = true;
                    break;
                case BaseMethodDeclarationSyntax method:
                    if (!method.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        return;
                    }

                    sawStaticMember = true;
                    break;
                case BasePropertyDeclarationSyntax property:
                    if (!property.Modifiers.Any(SyntaxKind.StaticKeyword))
                    {
                        return;
                    }

                    sawStaticMember = true;
                    break;
                default:
                    return;
            }
        }

        if (!sawStaticMember)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0002, declaration.Identifier.GetLocation()));
    }
}
