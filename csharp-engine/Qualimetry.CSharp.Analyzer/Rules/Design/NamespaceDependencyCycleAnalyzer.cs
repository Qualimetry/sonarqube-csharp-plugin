using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Qualimetry.CSharp.Analyzer.Helpers;

namespace Qualimetry.CSharp.Analyzer.Rules.Design;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamespaceDependencyCycleAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Descriptors.QCS0206);

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
                var operation = operationContext.Operation;
                ISymbol? referenced = null;
                switch (operation)
                {
                    case IFieldReferenceOperation fieldReference:
                        referenced = fieldReference.Field;
                        break;
                    case IPropertyReferenceOperation propertyReference:
                        referenced = propertyReference.Property;
                        break;
                    case IEventReferenceOperation eventReference:
                        referenced = eventReference.Event;
                        break;
                    case IMethodReferenceOperation methodReference
                        when methodReference.Method.MethodKind != MethodKind.Constructor:
                        referenced = methodReference.Method;
                        break;
                    case IObjectCreationOperation objectCreation:
                        referenced = objectCreation.Constructor?.ContainingType;
                        break;
                    case IInvocationOperation invocation
                        when invocation.TargetMethod?.MethodKind != MethodKind.LocalFunction:
                        referenced = invocation.TargetMethod;
                        break;
                }

                if (referenced == null)
                {
                    return;
                }

                string? consumerNamespace = NamespaceGraphHelper.GetNamespaceName(
                    operationContext.ContainingSymbol?.ContainingNamespace);

                string? providerNamespace = NamespaceGraphHelper.GetNamespaceName(
                    referenced.ContainingType?.ContainingNamespace ?? referenced.ContainingNamespace);

                if (!NamespaceGraphHelper.ShouldIncludeNamespace(consumerNamespace)
                    || !NamespaceGraphHelper.ShouldIncludeNamespace(providerNamespace)
                    || NamespaceGraphHelper.IsAncestorOrDescendant(consumerNamespace!, providerNamespace!))
                {
                    return;
                }

                edges
                    .GetOrAdd(consumerNamespace!, _ => new ConcurrentDictionary<string, byte>(StringComparer.Ordinal))
                    [providerNamespace!] = 0;
                locations
                    .GetOrAdd(consumerNamespace!, _ => new ConcurrentBag<Location>())
                    .Add(operationContext.Operation.Syntax.GetLocation());
            },
            OperationKind.FieldReference,
            OperationKind.PropertyReference,
            OperationKind.EventReference,
            OperationKind.MethodReference,
            OperationKind.ObjectCreation,
            OperationKind.Invocation);

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
                    string reportNamespace = cycle[0];
                    if (!locations.TryGetValue(reportNamespace, out ConcurrentBag<Location>? cycleLocations))
                    {
                        continue;
                    }

                    Location? location = cycleLocations.FirstOrDefault(l => l.IsInSource);
                    if (location == null)
                    {
                        continue;
                    }

                    endContext.ReportDiagnostic(Diagnostic.Create(
                        Descriptors.QCS0206,
                        location,
                        cycleText));
                }
            });
    }
}
