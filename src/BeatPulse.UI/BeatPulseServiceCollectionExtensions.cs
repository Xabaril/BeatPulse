using BeatPulse.UI.Core.HostedService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeatPulse.UI
{
    public static class BeatPulseServiceCollectionExtensions
    {
        public static IServiceCollection AddBeatPulseUI(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, LivenessHostedService>();

            return services;
        }
    }
}
