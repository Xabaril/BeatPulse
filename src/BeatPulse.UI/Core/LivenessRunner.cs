using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    class LivenessRunner
        : ILivenessRunner
    {
        private readonly LivenessDb _context;
        private readonly ILivenessFailureNotifier _failureNotifier;
        private readonly ILogger<LivenessRunner> _logger;
        private readonly BeatPulseSettings _settings;

        public LivenessRunner(LivenessDb context, ILivenessFailureNotifier failureNotifier, IOptions<BeatPulseSettings> settings, ILogger<LivenessRunner> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _failureNotifier = failureNotifier ?? throw new ArgumentNullException(nameof(failureNotifier));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings.Value ?? new BeatPulseSettings();
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            _logger.LogDebug("LivenessRuner is on run method.");

            var liveness = await _context.LivenessConfiguration
                    .ToListAsync();

            foreach (var item in liveness)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("LivenessRunner Run is cancelled.");

                    break;
                }

                var executionHistory = await EvaluateLiveness(item);

                await SaveExecutionHistory(_context, executionHistory);

                if (!executionHistory.IsHealthy)
                {
                    _logger.LogWarning($"LivenessRuner notify liveness failure for {item.LivenessUri}.");

                    await _failureNotifier.NotifyFailure(item.LivenessName, executionHistory.Result);
                }
            }

            _logger.LogDebug("LivenessRuner run is completed.");
        }

        private async Task<LivenessExecutionHistory> EvaluateLiveness(LivenessConfiguration livenessConfiguration)
        {
            var (uri, name) = livenessConfiguration;

            try
            {
                var response = await new HttpClient()
                    .GetAsync(uri);

                var success = response.IsSuccessStatusCode;

                var content = await response.Content
                    .ReadAsStringAsync();

                return new LivenessExecutionHistory()
                {
                    ExecutedOn = DateTime.UtcNow,
                    IsHealthy = success,
                    LivenessUri = uri,
                    LivenessName = name,
                    Result = content
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "LivenessRunner EvaluateLiveness throw the exception.");

                return new LivenessExecutionHistory()
                {
                    ExecutedOn = DateTime.UtcNow,
                    IsHealthy = false,
                    LivenessUri = uri,
                    LivenessName = name,
                    Result = exception.Message
                };
            }
        }

        public Task<List<LivenessExecutionHistory>> GetLatestRun(string livenessName, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(livenessName))
            {
                throw new ArgumentNullException(nameof(livenessName));
            }

            return _context.LivenessExecutionHistory
                .Where(lh=>lh.LivenessName == livenessName)
                .OrderByDescending(le => le.ExecutedOn)
                .Take(_settings.NumberOfItems)
                .ToListAsync(cancellationToken);
        }

        public Task<List<LivenessConfiguration>> GetLiveness(CancellationToken cancellationToken)
        {
            return _context.LivenessConfiguration
                .ToListAsync(cancellationToken);
        }

        private async Task SaveExecutionHistory(LivenessDb context, LivenessExecutionHistory history)
        {
            _logger.LogDebug("LivenessRuner save a new liveness execution history.");

            await _context.LivenessExecutionHistory
                .AddAsync(history);

            await _context.SaveChangesAsync();
        }
    }
}