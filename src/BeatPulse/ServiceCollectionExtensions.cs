using BeatPulse;
using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBeatPulse(this IServiceCollection services,Action<BeatPulseContext> setup = null)
        {
            var context = new BeatPulseContext();

            context.Add(
                new ActionHealthCheck(
                    BeatPulseKeys.BEATPULSE_SELF_NAME, 
                    BeatPulseKeys.BEATPULSE_SELF_SEGMENT,
                    httpContext => (BeatPulseKeys.BEATPULSE_SELF_MESSAGE, true)));

            setup?.Invoke(context);

            services.TryAddSingleton(context);
            services.TryAddSingleton<IBeatPulseService, BeatPulseService>();

            return services;
        }
    }
}
