using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Gen.Razor.Routes.BuildTasks.Abstract;

namespace Soenneker.Gen.Razor.Routes.BuildTasks;

///<inheritdoc cref="Abstract.IRazorRoutesGeneratorWriteRunner"/>
public sealed class RazorRoutesGeneratorWriteRunner : IRazorRoutesGeneratorWriteRunner
{
    private static readonly Regex _pageRegex = new(@"^\s*@page\s+""(?<route>[^""]+)""", RegexOptions.Compiled | RegexOptions.Multiline);

    public async ValueTask<int> Run(string[] args, CancellationToken cancellationToken)
    {
        Dictionary<string, string> map = ParseArgs(args);
        if (!map.TryGetValue("--projectDir", out string? projectDir) || string.IsNullOrWhiteSpace(projectDir))
            return Fail("Missing required --projectDir");

        projectDir = Path.GetFullPath(projectDir.Trim().Trim('"'));

        string blazorAppDir = GetFullPath(GetOptional(map, "--blazorAppDir") ?? projectDir, projectDir);
        string outputPath = GetFullPath(GetOptional(map, "--outputPath") ?? "routes.txt", projectDir);
        bool includeDynamicRoutes = TryParseBoolean(GetOptional(map, "--includeDynamicRoutes"), defaultValue: true);

        if (!Directory.Exists(blazorAppDir))
            return Fail($"Blazor app directory not found: {blazorAppDir}");

        try
        {
            List<string> routes = await DiscoverRoutes(blazorAppDir, includeDynamicRoutes, cancellationToken);
            await WriteRoutes(outputPath, routes, cancellationToken);
            Console.WriteLine($"Generated Razor routes with {routes.Count} route(s) at {outputPath}");
        }
        catch (Exception e)
        {
            return Fail($"Failed to generate Razor routes: {e.Message}");
        }

        return 0;
    }

    private static async ValueTask<List<string>> DiscoverRoutes(string blazorAppDir, bool includeDynamicRoutes, CancellationToken cancellationToken)
    {
        var routes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (string file in EnumerateRazorFiles(blazorAppDir))
        {
            cancellationToken.ThrowIfCancellationRequested();

            string content = await File.ReadAllTextAsync(file, cancellationToken);
            MatchCollection routeMatches = _pageRegex.Matches(content);

            foreach (Match routeMatch in routeMatches)
            {
                string route = routeMatch.Groups["route"].Value.Trim();
                if (route.Length == 0)
                    continue;

                if (!includeDynamicRoutes && IsDynamicRoute(route))
                    continue;

                routes.Add(route);
            }
        }

        return routes.OrderBy(route => route, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static IEnumerable<string> EnumerateRazorFiles(string directory)
    {
        foreach (string file in Directory.EnumerateFiles(directory, "*.razor", SearchOption.TopDirectoryOnly))
        {
            yield return file;
        }

        foreach (string childDirectory in Directory.EnumerateDirectories(directory))
        {
            string name = Path.GetFileName(childDirectory);
            if (IsExcludedDirectoryName(name))
                continue;

            foreach (string file in EnumerateRazorFiles(childDirectory))
            {
                yield return file;
            }
        }
    }

    private static async ValueTask WriteRoutes(string outputPath, IReadOnlyCollection<string> routes, CancellationToken cancellationToken)
    {
        string? outputDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        string text = routes.Count == 0 ? "" : string.Join(Environment.NewLine, routes) + Environment.NewLine;
        await File.WriteAllTextAsync(outputPath, text, new UTF8Encoding(false), cancellationToken);
    }

    private static string GetFullPath(string path, string basePath)
    {
        path = path.Trim().Trim('"');
        return Path.IsPathRooted(path) ? Path.GetFullPath(path) : Path.GetFullPath(Path.Combine(basePath, path));
    }

    private static bool IsDynamicRoute(string route)
    {
        return route.Contains('{', StringComparison.Ordinal) || route.Contains('*', StringComparison.Ordinal);
    }

    private static bool IsExcludedDirectoryName(string name)
    {
        return name.Equals("obj", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("node_modules", StringComparison.OrdinalIgnoreCase) ||
               name.Equals(".git", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetOptional(IReadOnlyDictionary<string, string> map, string key)
    {
        return map.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value) ? value.Trim().Trim('"') : null;
    }

    private static bool TryParseBoolean(string? value, bool defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        value = value.Trim().Trim('"');
        return bool.TryParse(value, out bool result) ? result : defaultValue;
    }

    private static Dictionary<string, string> ParseArgs(string[] args)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--", StringComparison.Ordinal) && i + 1 < args.Length)
            {
                map[args[i]] = args[i + 1];
                i++;
            }
        }
        return map;
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        return 1;
    }
}
