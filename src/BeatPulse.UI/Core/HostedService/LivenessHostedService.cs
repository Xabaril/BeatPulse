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

        private Timer _livenessTask;

        public LivenessHostedService(IServiceProvider provider, ILogger<LivenessHostedService> logger)
        {
            _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(provider));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if ( _livenessTask != null )
            {
                _livenessTask = new Timer(Execute, null, 1000, 1000);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_livenessTask != null)
            {
                _livenessTask.Change(Timeout.Infinite, Timeout.Infinite);
                _livenessTask.Dispose();

                _livenessTask = null;
            }

            return Task.CompletedTask;
            
        }

        void Execute(object state)
        {
            _logger.LogInformation("Executing LivenessHostedSErvice");
        }
    }
}
