using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.HostedService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeatPulse.UI
{
    public static class BeatPulseServiceCollectionExtensions
    {
        public static IServiceCollection AddBeatPulseUI(this IServiceCollection services)
        {
            services.AddOptions();

            services.AddSingleton<IHostedService, LivenessHostedService>();
            services.AddSingleton<ILivenessFailureNotifier, LivenessFailureNotifier>();

            services.AddScoped<ILivenessRunner, LivenessRunner>();

            services.AddDbContext<LivenessDb>(db =>
            {
                db.UseSqlite("Data Source=livenesdb");
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

                await db.Database.MigrateAsync();

                var configurationSection = new LivenessConfigurationSection();

                configuration.Bind(Globals.BEATPULSEUI_SECTION_SETTING_KEY, configurationSection);

                var liveness = configurationSection.Liveness?.Select(s => new LivenessConfiguration()
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

        class LivenessConfigurationSection
        {
            public List<LivenessConfigurationSetting> Liveness { get; set; }
        }

        class LivenessConfigurationSetting
        {
            public string Name { get; set; }

            public string Uri { get; set; }
        }
    }
}