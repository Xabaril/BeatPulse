using BeatPulse;
using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection;
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
                new ActionHealthCheck("self", BeatPulseKeys.BEATPULSE_SELF_SEGMENT, httpContext => true));

            setup?.Invoke(context);

            services.TryAddSingleton(context);
            services.TryAddSingleton<IBeatPulseService, BeatPulseService>();

            return services;
        }
    }
}
