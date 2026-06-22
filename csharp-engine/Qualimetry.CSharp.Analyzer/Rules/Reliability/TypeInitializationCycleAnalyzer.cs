using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Qualimetry.CSharp.Analyzer.Helpers;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TypeInitializationCycleAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Descriptors.QCS0207);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var edges = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>(StringComparer.Ordinal);
        var locations = new ConcurrentDictionary<string, ConcurrentBag<Location>>(StringComparer.Ordinal);

        context.RegisterOperationAction(
            operationContext =>
            {
                if (operationContext.Operation is not IFieldInitializerOperation initializer)
                {
                    return;
                }

                foreach (IFieldSymbol field in initializer.InitializedFields)
                {
                    if (!field.IsStatic)
                    {
                        continue;
                    }

                    string sourceKey = GetFieldKey(field);
                    foreach (IFieldReferenceOperation reference in initializer.Value.DescendantsAndSelf().OfType<IFieldReferenceOperation>())
                {
                    if (!reference.Field.IsStatic)
                    {
                        continue;
                    }

                    string targetKey = GetFieldKey(reference.Field);
                    if (string.Equals(sourceKey, targetKey, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    edges.GetOrAdd(sourceKey, _ => new ConcurrentDictionary<string, byte>(StringComparer.Ordinal))[targetKey] = 0;
                    locations.GetOrAdd(sourceKey, _ => new ConcurrentBag<Location>())
                        .Add(reference.Syntax.GetLocation());
                    }
                }
            },
            OperationKind.FieldInitializer);

        context.RegisterOperationAction(
            operationContext =>
            {
                if (operationContext.Operation is not IBlockOperation block
                    || operationContext.ContainingSymbol is not IMethodSymbol { MethodKind: MethodKind.StaticConstructor } staticConstructor)
                {
                    return;
                }

                string sourceKey = GetTypePrefix(staticConstructor.ContainingType) + "..cctor";
                foreach (IFieldReferenceOperation reference in block.Descendants().OfType<IFieldReferenceOperation>())
                {
                    if (!reference.Field.IsStatic)
                    {
                        continue;
                    }

                    string targetKey = GetFieldKey(reference.Field);
                    edges.GetOrAdd(sourceKey, _ => new ConcurrentDictionary<string, byte>(StringComparer.Ordinal))[targetKey] = 0;
                    locations.GetOrAdd(sourceKey, _ => new ConcurrentBag<Location>())
                        .Add(reference.Syntax.GetLocation());
                }
            },
            OperationKind.Block);

        context.RegisterCompilationEndAction(
            endContext =>
            {
                var adjacency = edges.ToDictionary(
                    pair => pair.Key,
                    pair => new HashSet<string>(pair.Value.Keys, StringComparer.Ordinal),
                    StringComparer.Ordinal);

                foreach (IReadOnlyList<string> cycle in DependencyGraphHelper.FindCycles(adjacency))
                {
                    if (cycle.Count < 2)
                    {
                        continue;
                    }

                    string cycleText = string.Join(" -> ", cycle);
                    string reportNode = cycle[0];
                    if (!locations.TryGetValue(reportNode, out ConcurrentBag<Location>? cycleLocations))
                    {
                        continue;
                    }

                    Location? location = cycleLocations.FirstOrDefault(l => l.IsInSource);
                    if (location == null)
                    {
                        continue;
                    }

                    endContext.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.QCS0207,
                        location,
                        cycleText));
                }
            });
    }

    private static string GetTypePrefix(INamedTypeSymbol type) =>
        type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

    private static string GetFieldKey(IFieldSymbol field) =>
        $"{GetTypePrefix(field.ContainingType)}.{field.Name}";
}
