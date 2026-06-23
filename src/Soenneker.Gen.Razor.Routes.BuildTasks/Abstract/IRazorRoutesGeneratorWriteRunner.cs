using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Gen.Razor.Routes.BuildTasks.Abstract;

public interface IRazorRoutesGeneratorWriteRunner
{
    ValueTask<int> Run(string[] args, CancellationToken cancellationToken);
}
