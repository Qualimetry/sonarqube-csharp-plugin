using System;
using System.Collections.Generic;

namespace Qualimetry.CSharp.Analyzer.Helpers;

internal static class DependencyGraphHelper
{
    internal static IReadOnlyList<IReadOnlyList<string>> FindCycles(
        IReadOnlyDictionary<string, HashSet<string>> adjacency)
    {
        var cycles = new List<IReadOnlyList<string>>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var stack = new HashSet<string>(StringComparer.Ordinal);
        var path = new List<string>();

        foreach (string node in adjacency.Keys)
        {
            Visit(node, adjacency, visited, stack, path, cycles);
        }

        return cycles;
    }

    private static void Visit(
        string node,
        IReadOnlyDictionary<string, HashSet<string>> adjacency,
        HashSet<string> visited,
        HashSet<string> stack,
        List<string> path,
        List<IReadOnlyList<string>> cycles)
    {
        if (stack.Contains(node))
        {
            int start = path.IndexOf(node);
            if (start >= 0)
            {
                var cycle = new List<string>();
                for (int i = start; i < path.Count; i++)
                {
                    cycle.Add(path[i]);
                }

                cycle.Add(node);
                cycles.Add(cycle);
            }

            return;
        }

        if (visited.Contains(node))
        {
            return;
        }

        visited.Add(node);
        stack.Add(node);
        path.Add(node);

        if (adjacency.TryGetValue(node, out HashSet<string>? edges))
        {
            foreach (string next in edges)
            {
                Visit(next, adjacency, visited, stack, path, cycles);
            }
        }

        path.RemoveAt(path.Count - 1);
        stack.Remove(node);
    }
}
