using BeatPulse;
using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseBeatPulse(this IWebHostBuilder hostBuilder, Action<BeatPulseOptions> optionsAction = null)
        {
            var options = new BeatPulseOptions();
            optionsAction?.Invoke(options);

            hostBuilder.ConfigureServices(defaultServices =>
            {
                defaultServices.AddSingleton<IStartupFilter>(new BeatPulseFilter(options));
            });

            return hostBuilder;
        }
    }
}
