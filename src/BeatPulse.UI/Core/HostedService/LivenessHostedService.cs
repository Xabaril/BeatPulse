using BeatPulse.UI.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core.HostedService
{
    class LivenessHostedService
        : IHostedService
    {
        private readonly ILogger<LivenessHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private Task _executingTask;


        public LivenessHostedService(IServiceProvider provider, ILogger<LivenessHostedService> logger)
        {
            _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(provider));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteASync(cancellationToken);

            if (_executingTask.IsCompleted)
            {
                //token cancelled just
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        async Task ExecuteASync(CancellationToken cancellationToken)
        {
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Executing LivenessHostedSErvice");

                await PerformLivenessEvaluation(scopeFactory, cancellationToken);

                await Task.Delay(5 * 1000);
            }
        }

        async Task PerformLivenessEvaluation(IServiceScopeFactory scopeFactory,CancellationToken cancellationToken)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<LivenessContext>();

                var liveness = await context.LivenessConfiguration
                    .ToListAsync();

                foreach (var item in liveness)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var excutionHistory = await EvaluateLiveness(item);

                    await SaveExecutionHistory(context, excutionHistory);

                    if (!excutionHistory.IsHealthy)
                    {
                        await NotifyFailure(item.WebHookNotificationUri, excutionHistory.Result);
                    }
                }
            }
        }

        async Task<LivenessExecutionHistory> EvaluateLiveness(LivenessConfiguration livenessConfiguration)
        {
            var hc = livenessConfiguration.LivenessUri;

            var response = await new HttpClient()
                .GetAsync(hc);

            var success = response.IsSuccessStatusCode;
            var content = await response.Content.ReadAsStringAsync();

            return new LivenessExecutionHistory()
            {
                ExecutedOn = DateTime.UtcNow,
                IsHealthy = success,
                LivenessUri = hc,
                Result = content
            };
        }

        async Task SaveExecutionHistory(LivenessContext context, LivenessExecutionHistory history)
        {
            await context.LivenessExecutionHistory
                .AddAsync(history);

            await context.SaveChangesAsync();
        }

        async Task NotifyFailure(string webHook, string content)
        {
            return Task.CompletedTask;
        }
    }
}
