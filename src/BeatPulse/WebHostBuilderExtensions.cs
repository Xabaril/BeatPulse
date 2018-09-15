using BeatPulse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseBeatPulse(this IWebHostBuilder hostBuilder, Action<BeatPulseOptions> setup = null)
        {
            hostBuilder.ConfigureServices(defaultServices =>
            {
                var options = new BeatPulseOptionsConfiguration();
                var configuration = defaultServices.BuildServiceProvider().GetRequiredService<IConfiguration>();
                configuration.Bind(BeatPulseKeys.BEATPULSE_OPTIONS_SETTING_KEY, options);
                
                setup?.Invoke(options);
                defaultServices.AddSingleton<IStartupFilter>(new BeatPulseFilter(options));
            });

            return hostBuilder;
        }       
    }
}
