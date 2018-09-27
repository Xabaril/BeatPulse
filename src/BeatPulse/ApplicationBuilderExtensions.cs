using BeatPulse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add BeatPulse middleware into the request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="setup">Provide delegate to configure BeatPulse options like timeout, path, detailed errors etc.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        public static IApplicationBuilder UseBeatPulse(this IApplicationBuilder app, Action<BeatPulseOptions> setup = null)
        {
            var options = new BeatPulseOptionsConfiguration();

            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            configuration.Bind(BeatPulseKeys.BEATPULSE_OPTIONS_SETTING_KEY, options);

            setup?.Invoke(options);

            app.MapWhen(context => context.IsBeatPulseRequest(options), appBuilder =>
            {
                appBuilder.UseMiddleware<BeatPulseMiddleware>(options);
            });

            return app;
        }

        /// <summary>
        /// Add BeatPulse middleware into the request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="setup">Provide delegate to configure BeatPulse options like timeout, path, detailed errors etc.</param>
        /// <param name="usePipeline">A provided delegate to configure specified middlewares to be executed on BeatPulse requests.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        public static IApplicationBuilder UseBeatPulse(this IApplicationBuilder app, Action<BeatPulseOptions> setup, Action<IApplicationBuilder> usePipeline)
        {
            var options = new BeatPulseOptionsConfiguration();

            var configuration = app.ApplicationServices.GetService<IConfiguration>();
            configuration.Bind(BeatPulseKeys.BEATPULSE_OPTIONS_SETTING_KEY, options);

            setup?.Invoke(options);

            app.MapWhen(context => context.IsBeatPulseRequest(options), appBuilder =>
            {
                usePipeline(appBuilder);

                appBuilder.UseMiddleware<BeatPulseMiddleware>(options);
            });

            return app;
        }
    }
}
