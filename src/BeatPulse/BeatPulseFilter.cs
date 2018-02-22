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
                builder.UseMiddleware<BeatPulseMiddleware>(_options);
                next(builder);
            };
        }
    }
}
