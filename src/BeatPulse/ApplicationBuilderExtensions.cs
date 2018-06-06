using BeatPulse;
using Microsoft.AspNetCore.Http;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseBeatPulse(this IApplicationBuilder app,Action<BeatPulseOptions> setup)
        {
            var options = new BeatPulseOptions();
            setup?.Invoke(options);

            app.MapWhen(context => context.IsBeatPulseRequest(options), appBuilder =>
            {
                appBuilder.UseMiddleware<BeatPulseMiddleware>(options);
            });

            return app;
        }
    }
}
