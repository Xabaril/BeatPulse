﻿using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core.Data;
using BeatPulse.UI.Core.Notifications;
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
        private static HttpClient _httpClient;

        private readonly LivenessDb _context;
        private readonly ILivenessFailureNotifier _failureNotifier;
        private readonly BeatPulseSettings _settings;
        private readonly ILogger<LivenessRunner> _logger;
        
        static LivenessRunner() {
            _httpClient = new HttpClient();
        }

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
            using (_logger.BeginScope("LivenessRuner is on run method."))
            {
                var liveness = await _context.LivenessConfigurations
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                var evaluatedLiveness = await Task.WhenAll(liveness.ConvertAll(o => EvaluateLiveness(o, cancellationToken)));

                foreach (var (response, isHealthy, item) in evaluatedLiveness)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogDebug("LivenessRunner Run is cancelled.");

                        break;
                    }
                    
                    if (isHealthy && (await HasLivenessRecoveredFromFailure(item, cancellationToken)))
                    {
                        await _failureNotifier.NotifyWakeUp(item.LivenessName);
                    }
                    else if (!isHealthy)
                    {
                        await _failureNotifier.NotifyDown(item.LivenessName, GetFailedMessageFromContent(response));
                    }

                    await SaveExecutionHistory(item, response, isHealthy, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogDebug("LivenessRuner run is completed.");
            }
        }

        protected internal virtual Task<HttpResponseMessage> PerformRequest(string uri, CancellationToken cancellationToken)
        {
            return _httpClient.GetAsync(uri, cancellationToken);
        }

        private async Task<(string response, bool ishealthy, LivenessConfiguration livenessConfiguration)> EvaluateLiveness(
            LivenessConfiguration livenessConfiguration,
            CancellationToken cancellationToken)
        {
            var (uri, name) = livenessConfiguration;

            try
            {
                using (var response = await PerformRequest(uri, cancellationToken))
                {
                    var success = response.IsSuccessStatusCode;

                    var content = await response.Content
                        .ReadAsStringAsync();

                    return (content, success, livenessConfiguration);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "LivenessRunner EvaluateLiveness throw the exception.");

                return (exception.Message, false, livenessConfiguration);
            }
        }

        private async Task<LivenessExecution> GetLivenessExecution(LivenessConfiguration liveness, CancellationToken cancellationToken)
        {
            return await _context.LivenessExecutions
                .Include(le => le.History)
                .Where(le => le.LivenessName.Equals(liveness.LivenessName, StringComparison.InvariantCultureIgnoreCase))
                .SingleOrDefaultAsync(cancellationToken);
        }

        private async Task<bool> HasLivenessRecoveredFromFailure(LivenessConfiguration liveness, CancellationToken cancellationToken)
        {
            var previousLivenessExecution = await GetLivenessExecution(liveness, cancellationToken);
            if (previousLivenessExecution != null)
            {
                var previousStatus = (LivenessStatus)Enum.Parse(typeof(LivenessStatus), previousLivenessExecution.Status);
                return (previousStatus != LivenessStatus.Up);
            }

            return false;
        }

        private async Task SaveExecutionHistory(LivenessConfiguration liveness, string content, bool isHealthy, CancellationToken cancellationToken)
        {
            _logger.LogDebug("LivenessRuner save a new liveness execution history.");

            var livenessExecution = await GetLivenessExecution(liveness, cancellationToken);

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

                    livenessExecution.History.Add(new LivenessExecutionHistory()
                    {
                        On = lastExecutionTime,
                        Status = livenessExecution.Status
                    });

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

                livenessExecution = new LivenessExecution()
                {
                    IsHealthy = isHealthy,
                    LastExecuted = lastExecutionTime,
                    OnStateFrom = lastExecutionTime,
                    LivenessResult = content,
                    Status = currentStatusName,
                    LivenessName = liveness.LivenessName,
                    LivenessUri = liveness.LivenessUri,
                    DiscoveryService = liveness.DiscoveryService
                };

                await _context.LivenessExecutions
                    .AddAsync(livenessExecution, cancellationToken);
            }
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
                    var message = JsonConvert.DeserializeObject<OutputLivenessMessageResponse>(content);

                    if (message != null)
                    {
                        var selfLiveness = message.Checks
                            .Where(s => s.Name.Equals("self", StringComparison.InvariantCultureIgnoreCase))
                            .SingleOrDefault();

                        return (selfLiveness.IsHealthy) ? LivenessStatus.Degraded : LivenessStatus.Down;
                    }
                }
                catch
                {
                    //probably the request can't be performed (invalid domain,empty or unexpected message)
                    _logger.LogWarning($"The response from uri can't be parsed correctly. The response is {content}");
                }


                return LivenessStatus.Down;
            }
        }

        private string GetFailedMessageFromContent(string content)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<OutputLivenessMessageResponse>(content);

                var failedChecks = message.Checks
                    .Where(c => !c.IsHealthy)
                    .ToList();

                var failingLiveness = failedChecks.Select(f => f.Name)
                    .Aggregate((current, next) => $"{current},{next}");

                return $"There is at least {failedChecks.Count} liveness ({failingLiveness}) failing.";
            }
            catch
            {
                return content;
            }
        }

        public void Dispose()
        {
        }

        private class OutputLivenessMessageResponse
        {
            public IEnumerable<LivenessResultResponse> Checks { get; set; }

            public DateTime StartedAtUtc { get; set; }

            public DateTime EndAtUtc { get; set; }

            public int Code { get; set; }

            public string Reason { get; set; }

        }

        private class LivenessResultResponse
        {
            public string Name { get; set; }

            public string Message { get; set; }

            public string Exception { get; set; }

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
