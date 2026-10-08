using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Soenneker.Gen.Razor.Routes.Tests;

public sealed class NativeExecutableTests
{
    [Test]
    public async Task Native_executable_generates_expected_output(CancellationToken cancellationToken)
    {
        string? executable = Environment.GetEnvironmentVariable("RAZOR_NATIVE_TOOL");
        if (string.IsNullOrEmpty(executable)) { TUnit.Core.Skip.Test("Set RAZOR_NATIVE_TOOL to run native integration tests."); return; }
        string directory = Path.Combine(Path.GetTempPath(), "razor native " + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            async Task Run(string[] arguments)
            {
                var start = new ProcessStartInfo(executable) { UseShellExecute = false };
                foreach (string argument in arguments) start.ArgumentList.Add(argument);
                using var process = Process.Start(start) ?? throw new Exception("Native tool could not start.");
                await process.WaitForExitAsync(cancellationToken: cancellationToken);
                if (process.ExitCode != 0) throw new Exception("Native tool failed: " + process.ExitCode);
            }
            await File.WriteAllTextAsync(Path.Combine(directory, "Home.razor"), "@page \"/\"\n@page \"/about\"", cancellationToken: cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(directory, "Dynamic.razor"), "@page \"/items/{id:int}\"", cancellationToken: cancellationToken);
            string output = Path.Combine(directory, "routes.txt");
            string[] arguments = ["--projectDir", directory, "--outputPath", output, "--includeDynamicRoutes", "false"];
            await Run(arguments);
            string[] routes = await File.ReadAllLinesAsync(output, cancellationToken: cancellationToken);
            if (!routes.SequenceEqual(new[] { "/", "/about" })) throw new Exception("Unexpected native routes: " + string.Join(",", routes));
            string original = await File.ReadAllTextAsync(output, cancellationToken: cancellationToken);
            await Run(arguments);
            if (await File.ReadAllTextAsync(output, cancellationToken: cancellationToken) != original) throw new Exception("Native routes are not deterministic.");
            arguments[^1] = "true";
            await Run(arguments);
            if (!(await File.ReadAllLinesAsync(output, cancellationToken: cancellationToken)).Contains("/items/{id:int}")) throw new Exception("Dynamic route was omitted.");
        }
        finally { Directory.Delete(directory, recursive: true); }
    }
}
