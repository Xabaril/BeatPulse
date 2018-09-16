using BeatPulse;
using BeatPulse.Core.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IHealthChecksBuilder AddBeatPulse(this IServiceCollection services)
        {
            services.TryAddSingleton<IBeatPulseAuthenticationFilter, NoAuthenticationFilter>();

            return services.AddHealthChecks().AddDelegateCheck("_self", failureStatus: null, tags: new[] { BeatPulseKeys.BEATPULSE_SELF_SEGMENT }, check: () =>
            {
                return HealthCheckResult.Passed(description: BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE);
            });
        }
    }
}
