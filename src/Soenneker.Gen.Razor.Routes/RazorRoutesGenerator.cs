using Microsoft.CodeAnalysis;
namespace Soenneker.Gen.Razor.Routes;
    /// <summary>
    /// Initializes the razor routes generator so it is ready for use.
    /// </summary>
    /// <param name="context">Roslyn initialization context used to register the source generator.</param>
[Generator]
public sealed class RazorRoutesGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the Razor Routes Generator so it is ready for use.
    /// </summary>
    /// <param name="context">HTTP context containing the Authorization header.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
    }
}
