using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace BeatPulse
{
    class BeatPulseFilter
        : IStartupFilter
    {
        private string _beatPulsePath;

        public BeatPulseFilter(string beatPulsePath)
        {
            _beatPulsePath = beatPulsePath ?? throw new ArgumentNullException(nameof(beatPulsePath));
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<BeatPulseMiddleware>(_beatPulsePath);

                next(builder);
            };
        }
    }
}
