using BeatPulse;
using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostExtensions
    {
        public static IWebHostBuilder UseBeatPulse(this IWebHostBuilder hostBuilder,string path = BeatPulseKeys.DefaultBeatPulsePath)
        {
            hostBuilder.ConfigureServices(defaultServices =>
            {
                defaultServices.AddSingleton<IStartupFilter>(new BeatPulseFilter(path));
                defaultServices.AddScoped<IBeatPulseService, BeatPulseService>();
            });

            return hostBuilder;
        }
    }
}
