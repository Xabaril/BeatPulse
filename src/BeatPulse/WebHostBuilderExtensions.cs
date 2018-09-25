using BeatPulse;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseBeatPulse(this IWebHostBuilder hostBuilder, Action<BeatPulseOptions> setup = null)
        {
            var options = new BeatPulseOptions();
            setup?.Invoke(options);

            hostBuilder.ConfigureServices(defaultServices =>
            {              
                defaultServices.AddSingleton<IStartupFilter>(new BeatPulseFilter(options));
            });

            return hostBuilder;
        }       
    }
}
