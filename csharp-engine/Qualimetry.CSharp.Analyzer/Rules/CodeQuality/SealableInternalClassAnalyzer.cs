using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.CodeQuality;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SealableInternalClassAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0048);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var baseTypes = new ConcurrentDictionary<INamedTypeSymbol, byte>(SymbolEqualityComparer.Default);
        var candidates = new ConcurrentDictionary<INamedTypeSymbol, byte>(SymbolEqualityComparer.Default);

        context.RegisterSymbolAction(
            symbolContext =>
            {
                var type = (INamedTypeSymbol)symbolContext.Symbol;

                if (type.BaseType is { } baseType && baseType.SpecialType != SpecialType.System_Object)
                {
                    baseTypes.TryAdd(baseType.OriginalDefinition, 0);
                }

                if (IsCandidate(type))
                {
                    candidates.TryAdd(type, 0);
                }
            },
            SymbolKind.NamedType);

        context.RegisterCompilationEndAction(
            endContext =>
            {
                foreach (var candidate in candidates.Keys)
                {
                    if (baseTypes.ContainsKey(candidate.OriginalDefinition))
                    {
                        continue;
                    }

                    foreach (var location in candidate.Locations.Where(l => l.IsInSource))
                    {
                        endContext.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0048, location));
                    }
                }
            });
    }

    private static bool IsCandidate(INamedTypeSymbol type)
    {
        if (type.TypeKind != TypeKind.Class)
        {
            return false;
        }

        if (type.IsSealed || type.IsAbstract || type.IsStatic || type.IsRecord)
        {
            return false;
        }

        if (type.Locations.All(l => !l.IsInSource))
        {
            return false;
        }

        return EffectiveAccessibilityIsInternalOrLess(type);
    }

    private static bool EffectiveAccessibilityIsInternalOrLess(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.ContainingType)
        {
            if (current.DeclaredAccessibility == Accessibility.Public
                || current.DeclaredAccessibility == Accessibility.Protected
                || current.DeclaredAccessibility == Accessibility.ProtectedOrInternal)
            {
                return false;
            }
        }

        return true;
    }
}
