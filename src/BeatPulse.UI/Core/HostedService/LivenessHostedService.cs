using BeatPulse.UI.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core.HostedService
{
    class LivenessHostedService
        : IHostedService
    {
        private readonly ILogger<LivenessHostedService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly BeatPulseSettings _settings;

        private Task _executingTask;

        public LivenessHostedService(IServiceScopeFactory serviceScopeFactory, IOptions<BeatPulseSettings> settings, ILogger<LivenessHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _settings = settings.Value ?? new BeatPulseSettings();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteASync(cancellationToken);

            if (_executingTask.IsCompleted)
            {
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
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Executing LivenessHostedService.");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var runner = scope.ServiceProvider.GetRequiredService<ILivenessRunner>();
                    try
                    {
                        await runner.Run(cancellationToken);

                        _logger.LogDebug("LivenessHostedService executed succesfully.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("LivenessHostedService throw a error:", ex);
                    }
                }

                await Task.Delay(_settings.EvaluationTimeOnSeconds * 1000);
            }
        }
    }
}
