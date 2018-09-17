using BeatPulse;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Microsoft.AspNetCore.Hosting
{
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Add a StartupFilter that provides BeatPulse features.
        /// </summary>
        /// <param name="hostBuilder">The <see cref="IWebHostBuilder"/>.</param>
        /// <param name="setup">Provided delegate to configure BeatPulse options like timeout, path, detailed errors etc.</param>
        /// <returns>The <see cref="IWebHostBuilder"/> reference with BeatPulse features.</returns>
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
