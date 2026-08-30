using Microsoft.CodeAnalysis;
namespace Soenneker.Gen.Razor.Routes;

/// <summary>
/// Provides the analyzer entry point shipped with the Razor routes build package.
/// </summary>
[Generator]
public sealed class RazorRoutesGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the Razor Routes Generator so it is ready for use.
    /// </summary>
    /// <param name="context">Roslyn initialization context used to register generator pipelines.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
    }
}
