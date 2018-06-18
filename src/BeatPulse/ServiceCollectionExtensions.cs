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
            context.AddLiveness(BeatPulseKeys.BEATPULSE_SELF_NAME, opt =>
            {
                var selfLiveness = new ActionLiveness(
                    cancellationToken => Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true)));

                opt.UsePath(BeatPulseKeys.BEATPULSE_SELF_SEGMENT);
                opt.UseLiveness(selfLiveness);
            });

            setup?.Invoke(context);

            services.TryAddSingleton(context);
            services.TryAddSingleton<IBeatPulseService, BeatPulseService>();
            services.TryAddSingleton<IBeatPulseAuthenticationFilter, NoAuthenticationFilter>();

            return services;
        }
    }
}
