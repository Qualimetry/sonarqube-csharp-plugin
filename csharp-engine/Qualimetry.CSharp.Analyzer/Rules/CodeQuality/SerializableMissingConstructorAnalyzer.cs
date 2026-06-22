using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SerializableMissingConstructorAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0038);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not { } typeSymbol)
        {
            return;
        }

        if (!ImplementsSerializable(typeSymbol))
        {
            return;
        }

        if (HasDeserializationConstructor(typeSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            Descriptors.QCS0038,
            declaration.Identifier.GetLocation(),
            declaration.Identifier.ValueText));
    }

    private static bool ImplementsSerializable(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces.Any(i =>
            i.Name == "ISerializable"
            && i.ContainingNamespace?.ToDisplayString() == "System.Runtime.Serialization");
    }

    private static bool HasDeserializationConstructor(INamedTypeSymbol typeSymbol)
    {
        foreach (var constructor in typeSymbol.InstanceConstructors)
        {
            if (constructor.Parameters.Length == 2
                && constructor.Parameters[0].Type.Name == "SerializationInfo"
                && constructor.Parameters[1].Type.Name == "StreamingContext")
            {
                return true;
            }
        }

        return false;
    }
}
