using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.HostedService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BeatPulse.UI
{
    public static class BeatPulseServiceCollectionExtensions
    {
        public static IServiceCollection AddBeatPulseUI(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider()
                .GetService<IConfiguration>();

            services.AddOptions();
            services.Configure<BeatPulseSettings>((settings) =>
            {
                configuration.Bind(BeatPulseUIKeys.BEATPULSEUI_SECTION_SETTING_KEY, settings);
            });

            services.AddSingleton<IHostedService, LivenessHostedService>();
            services.AddSingleton<ILivenessFailureNotifier, LivenessFailureNotifier>();
            services.AddScoped<ILivenessRunner, LivenessRunner>();
            services.AddDbContext<LivenessDb>(db =>
            {
                db.UseSqlite("Data Source=livenessdb");
            });

            var serviceProvider = services.BuildServiceProvider();

            CreateDatabase(serviceProvider).Wait();

            return services;
        }

        static async Task CreateDatabase(IServiceProvider serviceProvider)
        {
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider
                    .GetService<LivenessDb>();

                var configuration = scope.ServiceProvider
                    .GetService<IConfiguration>();

                var settings = scope.ServiceProvider
                    .GetService<IOptions<BeatPulseSettings>>();

                await db.Database.EnsureDeletedAsync();

                await db.Database.MigrateAsync();

                var liveness = settings.Value?
                    .Liveness?
                    .Select(s => new LivenessConfiguration()
                    {
                        LivenessName = s.Name,
                        LivenessUri = s.Uri
                    });

                if (liveness != null
                    &&
                    liveness.Any())
                {

                    await db.LivenessConfiguration
                        .AddRangeAsync(liveness);

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}