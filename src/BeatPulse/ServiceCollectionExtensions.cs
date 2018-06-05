using BeatPulse;
using BeatPulse.Core;
using BeatPulse.Core.Authentication;
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
            context.AddLiveness(new ActionLiveness(
                BeatPulseKeys.BEATPULSE_SELF_NAME,
                BeatPulseKeys.BEATPULSE_SELF_SEGMENT,
                (httpContext, cancellationToken) => Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true))));

            setup?.Invoke(context);

            services.AddCors();
            services.TryAddSingleton(context);
            services.TryAddSingleton<IBeatPulseService, BeatPulseService>();
            services.TryAddSingleton<IBeatPulseAuthenticationFilter, NoAuthenticationFilter>();

            return services;
        }
    }
}
