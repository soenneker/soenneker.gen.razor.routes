using System.Threading;
using System.Threading.Tasks;
namespace Soenneker.Gen.Razor.Routes.BuildTasks.Abstract;
public interface IRazorRoutesGeneratorWriteRunner
{
    /// <summary>
    /// Runs razor Routes Generator Write Runner for the Razor Routes Generator Write Runner.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested value.</returns>
    ValueTask<int> Run(string[] args, CancellationToken cancellationToken);
}
