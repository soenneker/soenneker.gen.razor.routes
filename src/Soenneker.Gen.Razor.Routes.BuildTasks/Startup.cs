using Microsoft.Extensions.DependencyInjection;
using Soenneker.Gen.Razor.Routes.BuildTasks.Abstract;
using Soenneker.Utils.Directory.Registrars;
using Soenneker.Utils.File.Registrars;
namespace Soenneker.Gen.Razor.Routes.BuildTasks;
public static class Startup
{
    /// <summary>
    /// Registers the services required by the application host.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddDirectoryUtilAsSingleton()
                .AddFileUtilAsSingleton()
                .AddSingleton<IRazorRoutesGeneratorWriteRunner, RazorRoutesGeneratorWriteRunner>();
        services.AddHostedService<ConsoleHostedService>();
    }
}
