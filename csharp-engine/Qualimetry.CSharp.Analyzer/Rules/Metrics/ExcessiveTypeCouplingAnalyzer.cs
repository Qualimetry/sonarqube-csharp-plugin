using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
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

        var semanticModel = context.SemanticModel;
        var referenced = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var type in CollectSignatureTypes(declaration))
        {
            CollectNamedTypes(semanticModel.GetTypeInfo(type).Type, referenced);
        }

        if (semanticModel.GetDeclaredSymbol(declaration) is INamedTypeSymbol ownSymbol)
        {
            referenced.Remove(ownSymbol.OriginalDefinition);
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

    private static void CollectNamedTypes(ITypeSymbol? symbol, HashSet<INamedTypeSymbol> accumulator)
    {
        switch (symbol)
        {
            case null:
            case ITypeParameterSymbol:
                return;
            case IArrayTypeSymbol array:
                CollectNamedTypes(array.ElementType, accumulator);
                return;
            case IPointerTypeSymbol pointer:
                CollectNamedTypes(pointer.PointedAtType, accumulator);
                return;
            case INamedTypeSymbol named:
                if (!IsPredefined(named))
                {
                    accumulator.Add(named.OriginalDefinition);
                }

                foreach (var argument in named.TypeArguments)
                {
                    CollectNamedTypes(argument, accumulator);
                }

                return;
        }
    }

    private static bool IsPredefined(INamedTypeSymbol named)
    {
        switch (named.SpecialType)
        {
            case SpecialType.System_Void:
            case SpecialType.System_Boolean:
            case SpecialType.System_Char:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Decimal:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_String:
            case SpecialType.System_Object:
            case SpecialType.System_Nullable_T:
                return true;
            default:
                return false;
        }
    }
}
