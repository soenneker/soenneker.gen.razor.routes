using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Gen.Razor.Routes.BuildTasks;
using Soenneker.Tests.Unit;

namespace Soenneker.Gen.Razor.Routes.Tests;

public sealed class RazorRoutesGeneratorTests : UnitTest
{
    [Test]
    public async ValueTask Generates_routes_txt_from_configured_blazor_app()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "soenneker-razor-routes-" + Guid.NewGuid().ToString("N"));
        string consumerDir = Path.Combine(tempDir, "consumer");
        string blazorAppDir = Path.Combine(tempDir, "client");
        string pagesDir = Path.Combine(blazorAppDir, "Components", "Pages");
        string outputPath = Path.Combine("generated", "routes.txt");
        string outputFullPath = Path.Combine(consumerDir, outputPath);

        try
        {
            Directory.CreateDirectory(consumerDir);
            Directory.CreateDirectory(pagesDir);
            Directory.CreateDirectory(Path.Combine(blazorAppDir, "obj"));

            await File.WriteAllTextAsync(Path.Combine(pagesDir, "Home.razor"), """
                @page "/"
                @page "/home"
                <h1>Home</h1>
                """);

            await File.WriteAllTextAsync(Path.Combine(pagesDir, "Products.razor"), """
                @page "/products/{id:int}"
                <h1>Product</h1>
                """);

            await File.WriteAllTextAsync(Path.Combine(pagesDir, "Search.razor"), """
                @page "/search"
                <h1>Search</h1>
                """);

            await File.WriteAllTextAsync(Path.Combine(blazorAppDir, "obj", "Generated.razor"), """
                @page "/obj-generated"
                """);

            var runner = new RazorRoutesGeneratorWriteRunner();
            int exitCode = await runner.Run(new[]
            {
                "--projectDir", consumerDir,
                "--blazorAppDir", blazorAppDir,
                "--outputPath", outputPath
            }, CancellationToken.None);

            if (exitCode != 0)
                throw new InvalidOperationException($"Runner exited with {exitCode}");

            string[] routes = (await File.ReadAllLinesAsync(outputFullPath)).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
            string[] expected = { "/", "/home", "/products/{id:int}", "/search" };

            if (!routes.SequenceEqual(expected, StringComparer.Ordinal))
                throw new InvalidOperationException($"Unexpected routes:{Environment.NewLine}{string.Join(Environment.NewLine, routes)}");
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }
}
