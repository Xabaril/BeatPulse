using BeatPulse;
using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseBeatPulse(this IWebHostBuilder hostBuilder,string path = BeatPulseKeys.BEATPULSE_DEFAULT_PATH)
        {
            hostBuilder.ConfigureServices(defaultServices =>
            {
                defaultServices.AddSingleton<IStartupFilter>(new BeatPulseFilter(path));
            });

            return hostBuilder;
        }
    }
}
