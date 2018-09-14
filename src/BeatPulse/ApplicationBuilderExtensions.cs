using BeatPulse;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
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
