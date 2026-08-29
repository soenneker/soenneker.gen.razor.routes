using Microsoft.Extensions.DependencyInjection;
using Soenneker.Gen.Razor.Routes.BuildTasks.Abstract;
namespace Soenneker.Gen.Razor.Routes.BuildTasks;
public static class Startup
{
    /// <summary>
    /// Registers the services required by the application host.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IRazorRoutesGeneratorWriteRunner, RazorRoutesGeneratorWriteRunner>();
        services.AddHostedService<ConsoleHostedService>();
    }
}
