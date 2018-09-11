﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public class BeatPulseService
        : IBeatPulseService
    {
        private readonly BeatPulseContext _beatPulseContext;
        private readonly ILogger<BeatPulseService> _logger;

        public BeatPulseService(BeatPulseContext beatPulseContext, IServiceProvider serviceProvider, ILogger<BeatPulseService> logger)
        {
            _beatPulseContext = beatPulseContext ?? throw new ArgumentNullException(nameof(beatPulseContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _beatPulseContext.UseServiceProvider(serviceProvider);
        }

        public async Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions options, HttpContext httpContext)
        {
            _logger.LogInformation($"BeatPulse is checking health on all registered liveness on [BeatPulsePath]/{path}.");

            var livenessResults = new List<LivenessResult>();

            foreach (var registration in _beatPulseContext.GetAllLiveness(path))
            {
                var liveness = _beatPulseContext
                    .CreateLivenessFromRegistration(registration);

                var executionContext = LivenessExecutionContext
                    .FromRegistration(registration, showDetailedErrors: options.DetailedErrors);

                var livenessResult = await RunLiveness(
                    liveness,
                    executionContext,
                    options);

                await RunTrackers(livenessResult);

                livenessResults.Add(livenessResult);

                if (!livenessResult.IsHealthy && !options.DetailedOutput)
                {
                    //if is unhealthy and not detailed options is true return inmediatly

                    _logger.LogWarning($"Liveness {executionContext.Name} is not healthy. Breaking liveness execution because detailed output is false.");

                    return livenessResults;
                }
            }

            return livenessResults;
        }

        Task RunTrackers(LivenessResult responses)
        {
            _logger.LogInformation("Sending liveness result to all configured trackers.");

            var trackerTasks = new List<Task>();

            if (_beatPulseContext.GetAllTrackers() != null)
            {
                foreach (var tracker in _beatPulseContext.GetAllTrackers())
                {
                    _logger.LogInformation($"Sending liveness result to tracker {tracker.GetType().FullName}.");

                    trackerTasks.Add(tracker.Track(responses));
                }
            }

            return Task.WhenAll(trackerTasks);
        }

        async Task<LivenessResult> RunLiveness(IBeatPulseLiveness liveness, LivenessExecutionContext executionContext, BeatPulseOptions options)
        {
            _logger.LogInformation($"Executing liveness {executionContext.Name}.");

            var livenessResult = new LivenessResult(executionContext.Name, executionContext.Path);
            livenessResult.StartCounter();

            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    var livenessTask = liveness.IsHealthy(executionContext, cancellationTokenSource.Token);

                    if (await Task.WhenAny(livenessTask, Task.Delay(options.Timeout, cancellationTokenSource.Token)) == livenessTask)
                    {
                        // The liveness is executed successfully and get the results
                        var (message, healthy) = await livenessTask;

                        livenessResult.StopCounter(message, healthy);

                        _logger.LogInformation($"The liveness {executionContext.Name} is executed.");
                    }
                    else
                    {
                        // The liveness is timeout ( from configured options)
                        _logger.LogWarning($"The liveness {executionContext.Name} return timeout, execution is cancelled.");

                        cancellationTokenSource.Cancel();

                        livenessResult.StopCounter(BeatPulseKeys.BEATPULSE_TIMEOUT_MESSAGE, false);
                    }
                }
            }
            catch (Exception ex)
            {
                // The uri executed is now well formed, dns not found
                // or any unexpected errror 

                _logger.LogError(ex, $"The liveness {executionContext.Name} is unhealthy.");

                var message = options.DetailedErrors
                    ? ex.Message : string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, executionContext.Name);

                livenessResult.StopCounter(message, false);
            }

            return livenessResult;
        }
    }
}
