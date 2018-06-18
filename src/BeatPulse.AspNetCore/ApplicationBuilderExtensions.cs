using BeatPulse;
using Microsoft.AspNetCore.Http;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeatPulse(this IApplicationBuilder app, Action<IBeatPulseOptions> setup)
        {
            var options = new BeatPulseOptions();
            setup?.Invoke(options);

            app.MapWhen(context => context.IsBeatPulseRequest(options), appBuilder =>
            {
                appBuilder.UseMiddleware<BeatPulseMiddleware>(options);
            });

            return app;
        }

        public static IApplicationBuilder UseBeatPulse(this IApplicationBuilder app, Action<IBeatPulseOptions> setup, Action<IApplicationBuilder> usePipeline)
        {
            var options = new BeatPulseOptions();
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
