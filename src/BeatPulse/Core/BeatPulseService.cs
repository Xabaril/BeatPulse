using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public class BeatPulseService
        : IBeatPulseService
    {
        private readonly BeatPulseContext _beatPulseContext;
        private readonly ILogger<BeatPulseService> _logger;
        private readonly IHostingEnvironment _environment;

        public BeatPulseService(BeatPulseContext beatPulseContext, IHostingEnvironment environment, IServiceProvider serviceProvider, ILogger<BeatPulseService> logger)
        {
            _beatPulseContext = beatPulseContext ?? throw new ArgumentNullException(nameof(beatPulseContext));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _beatPulseContext.UseServiceProvider(serviceProvider);
        }

        public async Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions beatPulseOptions, HttpContext httpContext)
        {
            _logger.LogInformation($"BeatPulse is checking health on [BeatPulsePath]/{path}");

            if (String.IsNullOrEmpty(path))
            {
                var livenessResults = new List<LivenessResult>();

                foreach (var registration in _beatPulseContext.GetAllLivenessRegistrations())
                {
                    var liveness = _beatPulseContext
                        .CreateLivenessFromRegistration(registration);

                    var livenessContext = LivenessExecutionContext
                        .FromRegistration(registration, _environment.IsDevelopment());

                    var livenessResult = await RunLiveness(
                        liveness,
                        livenessContext,
                        httpContext,
                        beatPulseOptions);

                    await RunTrackers(livenessResult);

                    livenessResults.Add(livenessResult);

                    if (!livenessResult.IsHealthy && !beatPulseOptions.DetailedOutput)
                    {
                        //if is unhealthy and not detailed options is true return inmediatly

                        _logger.LogWarning($"Liveness {livenessContext.Name} is not healthy");

                        return livenessResults;
                    }
                }

                return livenessResults;
            }
            else
            {
                var registration = _beatPulseContext
                    .FindLivenessRegistration(path);

                var liveness = _beatPulseContext
                    .CreateLivenessFromRegistration(registration);

                var livenessContext = LivenessExecutionContext.FromRegistration(registration, _environment.IsDevelopment());

                if (liveness != null)
                {
                    var livenessResult = await RunLiveness(liveness, 
                        livenessContext,
                        httpContext,
                        beatPulseOptions);

                    return new[] { livenessResult };
                }
            }

            return Enumerable.Empty<LivenessResult>();
        }

        Task RunTrackers(LivenessResult responses)
        {
            _logger.LogInformation("Sending liveness result to all configured trackers.");

            var trackerTasks = new List<Task>();

            if (_beatPulseContext.AllTrackers != null)
            {
                foreach (var tracker in _beatPulseContext.AllTrackers)
                {
                    _logger.LogInformation($"Sending liveness result to tracker {tracker.GetType().FullName}.");

                    trackerTasks.Add(tracker.Track(responses));
                }
            }

            return Task.WhenAll(trackerTasks);
        }

        async Task<LivenessResult> RunLiveness(IBeatPulseLiveness liveness, LivenessExecutionContext livenessContext, HttpContext httpContext, BeatPulseOptions beatPulseOptions)
        {
            _logger.LogInformation($"Executing liveness {livenessContext.Name}.");

            var livenessResult = new LivenessResult(livenessContext.Name, livenessContext.Path);

            livenessResult.StartCounter();

            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    var livenessTask = liveness.IsHealthy(httpContext, livenessContext, cancellationTokenSource.Token);

                    if (await Task.WhenAny(livenessTask, Task.Delay(beatPulseOptions.Timeout, cancellationTokenSource.Token)) == livenessTask)
                    {
                        // The liveness is executed successfully and get the results
                        var (message, healthy) = await livenessTask;

                        livenessResult.StopCounter(message, healthy);

                        _logger.LogInformation($"The liveness {livenessContext.Name} is executed.");
                    }
                    else
                    {
                        // The liveness is timeout ( from configured options)
                        _logger.LogWarning($"The liveness {livenessContext.Name} is timeout");

                        cancellationTokenSource.Cancel();
                        livenessResult.StopCounter(BeatPulseKeys.BEATPULSE_TIMEOUT_MESSAGE, false);
                    }
                }
            }
            catch (Exception ex)
            {
                // The uri executed is now well formed, dns not found
                // or any unexpected errror 

                _logger.LogError(ex, $"The liveness {livenessContext.Name} is unhealthy.");

                var message = _environment.IsDevelopment()
                    ? ex.Message : string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name);

                livenessResult.StopCounter(message, false);
            }

            return livenessResult;
        }
    }
}
