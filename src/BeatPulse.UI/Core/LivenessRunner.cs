using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly BeatPulseSettings _settings;
        private readonly ILogger<LivenessRunner> _logger;

        public LivenessRunner(LivenessDb context,
            ILivenessFailureNotifier failureNotifier,
            IOptions<BeatPulseSettings> settings,
            ILogger<LivenessRunner> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _failureNotifier = failureNotifier ?? throw new ArgumentNullException(nameof(failureNotifier));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

                var (content, isHealthy) = await EvaluateLiveness(item);

                await SaveExecutionHistory(_context, item, content, isHealthy);

                if (!isHealthy)
                {
                    _logger.LogWarning($"LivenessRuner notify liveness failure for {item.LivenessUri}.");

                    var lastNotification = _context.LivenessFailuresNotifications
                        .Where(lf => lf.LivenessName.Equals(item.LivenessName, StringComparison.InvariantCultureIgnoreCase))
                        .OrderByDescending(lf => lf.LastNotified)
                        .Take(1)
                        .SingleOrDefault();

                    if (lastNotification != null
                        &&
                        (DateTime.UtcNow - lastNotification.LastNotified).Seconds < _settings.MinimunSecondsBetweenFailureNotifications)
                    {
                        _logger.LogInformation("Notification is not performed becaused is already notified and the elapsed time is less than configured.");
                    }
                    else
                    {
                        await _failureNotifier.NotifyFailure(item.LivenessName, content);

                        var notification = new LivenessFailureNotification()
                        {
                            LivenessName = item.LivenessName,
                            LastNotified = DateTime.UtcNow
                        };

                        await SaveNotification(notification);

                        _logger.LogWarning("A new notification failure is created and sent.");
                    }
                }
            }

            _logger.LogDebug("LivenessRuner run is completed.");
        }

        public Task<LivenessExecutionHistory> GetLatestRun(string livenessName, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(livenessName))
            {
                throw new ArgumentNullException(nameof(livenessName));
            }

            return _context.LivenessExecutionHistory
                .Where(lh => lh.LivenessName == livenessName)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<LivenessConfiguration>> GetLiveness(CancellationToken cancellationToken)
        {
            return _context.LivenessConfiguration
                .ToListAsync(cancellationToken);
        }

        protected internal virtual Task<HttpResponseMessage> PerformRequest(string uri)
        {
            return new HttpClient()
                    .GetAsync(uri);
        }

        private async Task<(string content, bool ishealthy)> EvaluateLiveness(LivenessConfiguration livenessConfiguration)
        {
            var (uri, name) = livenessConfiguration;

            try
            {
                var response = await PerformRequest(uri);

                var success = response.IsSuccessStatusCode;

                var content = await response.Content
                    .ReadAsStringAsync();

                return (content, success);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "LivenessRunner EvaluateLiveness throw the exception.");

                return (exception.Message, false);
            }
        }

        private async Task SaveExecutionHistory(LivenessDb context, LivenessConfiguration liveness, string content, bool isHealthy)
        {
            _logger.LogDebug("LivenessRuner save a new liveness execution history.");

            var livenessExecution = await _context.LivenessExecutionHistory
                .Where(le => le.LivenessName == liveness.LivenessName && le.LivenessUri == liveness.LivenessUri)
                .SingleOrDefaultAsync();

            var currentStatus = GetDetailedStatusFromContent(isHealthy, content);
            var currentStatusName = Enum.GetName(typeof(LivenessStatus), currentStatus);
            var lastExecutionTime = DateTime.UtcNow;

            if (livenessExecution != null)
            {
                if (livenessExecution.Status.Equals(currentStatusName, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogDebug("LivenessExecutionHistory already exist and is in the same state, update the values.");

                    livenessExecution.LastExecuted = lastExecutionTime;
                    livenessExecution.LivenessResult = content;
                }
                else
                {
                    _logger.LogDebug("LivenessExecutionHistory already exist but on different state, update the values.");

                    livenessExecution.IsHealthy = isHealthy;
                    livenessExecution.OnStateFrom = lastExecutionTime;
                    livenessExecution.LastExecuted = lastExecutionTime;
                    livenessExecution.LivenessResult = content;
                    livenessExecution.Status = currentStatusName;
                }
            }
            else
            {
                _logger.LogDebug("LivenessExecutionHistory is a new liveness execution history.");

                livenessExecution = new LivenessExecutionHistory()
                {
                    IsHealthy = isHealthy,
                    LastExecuted = lastExecutionTime,
                    OnStateFrom = lastExecutionTime,
                    LivenessResult = content,
                    Status = currentStatusName,
                    LivenessName = liveness.LivenessName,
                    LivenessUri = liveness.LivenessUri,
                };

                await _context.LivenessExecutionHistory
                    .AddAsync(livenessExecution);
            }

            await _context.SaveChangesAsync();
        }

        private LivenessStatus GetDetailedStatusFromContent(bool isHealthy, string content)
        {
            if (isHealthy)
            {
                return LivenessStatus.Up;
            }
            else
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<OutputMessageResponse>(content);

                    if (message != null)
                    {
                        var selfLiveness = message.Checks
                            .Where(s => s.Name.Equals("self", StringComparison.InvariantCultureIgnoreCase))
                            .Single();

                        return (selfLiveness.IsHealthy) ? LivenessStatus.Degraded : LivenessStatus.Degraded;
                    }
                }
                catch (JsonReaderException)
                {
                    //probably the request can't be performed (invalid domain, etc)
                    _logger.LogWarning($"The response from uri can't be parsed correctly. The response is {content}");
                }


                return LivenessStatus.Down;
            }
        }

        private async Task SaveNotification(LivenessFailureNotification notification)
        {
            if (notification != null)
            {
                await _context.LivenessFailuresNotifications.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
        }

        private class OutputMessageResponse
        {
            public IEnumerable<LivenessResultResponse> Checks { get; set; }

            public DateTime StartedAtUtc { get; set; }

            public DateTime EndAtUtc { get; set; }

        }

        private class LivenessResultResponse
        {
            public string Name { get; set; }

            public string Message { get; set; }

            public long MilliSeconds { get; set; }

            public bool Run { get; set; }

            public string Path { get; set; }

            public bool IsHealthy { get; set; }
        }

        private enum LivenessStatus
        {
            Up = 0,
            Degraded = 1,
            Down = 2
        }
    }
}