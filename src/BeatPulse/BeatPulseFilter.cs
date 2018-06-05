using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace BeatPulse
{
    class BeatPulseFilter
        : IStartupFilter
    {
        private readonly BeatPulseOptions _options;

        public BeatPulseFilter(BeatPulseOptions options)
        {
            _options = options;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.MapWhen(context => context.IsBeatPulseRequest(_options), appBuilder =>
                {
                    if (_options.CorsEnabled)
                    {
                        appBuilder.UseCors(_options.CorsPolicyBuilder);
                    }

                    appBuilder.UseMiddleware<BeatPulseMiddleware>(_options);
                });

                next(builder);
            };
        }
    }
}
