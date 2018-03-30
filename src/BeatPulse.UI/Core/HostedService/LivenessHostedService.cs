using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
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

        private async Task ExecuteASync(CancellationToken cancellationToken)
        {
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Executing LivenessHostedSErvice");

                using (var scope = scopeFactory.CreateScope())
                {
                    var runner = scope.ServiceProvider
                        .GetRequiredService<ILivenessRunner>();

                    await runner.Run(cancellationToken);
                }

                await Task.Delay(5 * 1000);
            }
        }
    }
}
