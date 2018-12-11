using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.Discovery.K8S;
using BeatPulse.UI.Core.HostedService;
using BeatPulse.UI.Core.Notifications;
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

            services.AddScoped<ILivenessFailureNotifier, WebHookFailureNotifier>();
            services.AddScoped<ILivenessRunner, LivenessRunner>();
            services.AddDbContext<LivenessDb>(db =>
            {
                var connectionString = configuration["BeatPulse-UI:DatabaseConfiguration:ConnectionString"];
                if (string.IsNullOrWhiteSpace(connectionString) == false) {
                    db.UseSqlServer(connectionString);
                }
                else {
                    db.UseSqlite("Data Source=livenessdb");
                } 
            });

            var kubernetesDiscoveryOptions = new KubernetesDiscoveryOptions();
            configuration.Bind(BeatPulseUIKeys.BEATPULSEUI_KUBERNETES_DISCOVERY_SETTING_KEY, kubernetesDiscoveryOptions);

            if (kubernetesDiscoveryOptions.Enabled)
            {
                services.AddSingleton(kubernetesDiscoveryOptions);
                services.AddHostedService<KubernetesDiscoveryHostedService>();
            }

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

                if (Convert.ToBoolean(configuration["BeatPulse-UI:DatabaseConfiguration:EnsureHistory"]) == false){
                    await db.Database.EnsureDeletedAsync();
                }
               
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

                    await db.LivenessConfigurations
                        .AddRangeAsync(liveness);

                    await db.SaveChangesAsync();
                }
            }
        }
    }
}