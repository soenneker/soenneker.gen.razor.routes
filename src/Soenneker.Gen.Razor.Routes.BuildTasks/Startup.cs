using Microsoft.Extensions.DependencyInjection;
using Soenneker.Gen.Razor.Routes.BuildTasks.Abstract;

namespace Soenneker.Gen.Razor.Routes.BuildTasks;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IRazorRoutesGeneratorWriteRunner, RazorRoutesGeneratorWriteRunner>();
        services.AddHostedService<ConsoleHostedService>();
    }
}
