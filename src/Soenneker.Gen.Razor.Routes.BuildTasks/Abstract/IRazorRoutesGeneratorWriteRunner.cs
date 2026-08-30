using System.Threading;
using System.Threading.Tasks;
namespace Soenneker.Gen.Razor.Routes.BuildTasks.Abstract;
/// <summary>
/// Runs the Razor route discovery build task from its command-line arguments.
/// </summary>
public interface IRazorRoutesGeneratorWriteRunner
{
    /// <summary>
    /// Discovers Razor page directives and writes the configured route file.
    /// </summary>
    /// <param name="args">Generator command-line arguments supplied by the MSBuild target.</param>
    /// <param name="cancellationToken">Cancels discovery or output.</param>
    /// <returns>Zero when generation succeeds; otherwise a nonzero process exit code.</returns>
    ValueTask<int> Run(string[] args, CancellationToken cancellationToken);
}
