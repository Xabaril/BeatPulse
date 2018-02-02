using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace BeatPulse
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBeatPulse(this IServiceCollection services,Action<BeatPulseChecks> setup = null)
        {
            var beatPulseChecks = new BeatPulseChecks();

            setup?.Invoke(beatPulseChecks);

            services.TryAddSingleton(beatPulseChecks);
            services.TryAddSingleton<IBeatPulseService, BeatPulseService>();

            return services;
        }
    }
}
