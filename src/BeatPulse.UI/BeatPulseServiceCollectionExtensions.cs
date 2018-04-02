using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.HostedService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeatPulse.UI
{
    public static class BeatPulseServiceCollectionExtensions
    {
        public static IServiceCollection AddBeatPulseUI(this IServiceCollection services)
        {
            services.AddSingleton<IHostedService, LivenessHostedService>();
            services.AddSingleton<ILivenessFailureNotifier, LivenessFailureNotifier>();
            services.AddScoped<ILivenessRunner, LivenessRunner>();
            services.AddDbContext<LivenessContext>(db =>
            {
                db.UseSqlite("Data Source=livenesdb");
            });

            var context = services.BuildServiceProvider()
                .GetRequiredService<LivenessContext>();

            context.Database.Migrate();

            return services;
        }
    }
}
