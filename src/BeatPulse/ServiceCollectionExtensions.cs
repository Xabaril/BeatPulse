using BeatPulse;
using BeatPulse.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBeatPulse(this IServiceCollection services, Action<BeatPulseContext> setup = null)
        {
            var context = new BeatPulseContext();

            //add default self segment out of box
            context.Add(new ActionLiveness(
                BeatPulseKeys.BEATPULSE_SELF_NAME,
                BeatPulseKeys.BEATPULSE_SELF_SEGMENT,
                (httpContext,cancellationToken) => Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true))));

            setup?.Invoke(context);

            services.TryAddSingleton(context);
            services.TryAddSingleton<IBeatPulseService, BeatPulseService>();

            return services;
        }
    }
}
