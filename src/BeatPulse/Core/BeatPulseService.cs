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

        public BeatPulseService(BeatPulseContext context, IHostingEnvironment environment, ILogger<BeatPulseService> logger)
        {
            _beatPulseContext = context ?? throw new ArgumentNullException(nameof(context));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions options, HttpContext httpContext)
        {
            _logger.LogInformation($"BeatPulse is checking health on [BeatPulsePath]/{path}");

            if (String.IsNullOrEmpty(path))
            {
                var livenessResults = new List<LivenessResult>();

                foreach (var liveness in _beatPulseContext.AllLiveness)
                {
                    var healthCheckResult = await RunLiveness(liveness, options, httpContext);

                    this.Track(healthCheckResult);
                    
                    livenessResults.Add(healthCheckResult);

                    if (!healthCheckResult.IsHealthy && !options.DetailedOutput)
                    {
                        //if is unhealthy and not detailed options is true return inmediatly

                        _logger.LogWarning($"Liveness {liveness.Name} is not healthy");

                        return livenessResults;
                    }
                }

                return livenessResults;
            }
            else
            {
                var liveness = _beatPulseContext.FindLiveness(path);

                if (liveness != null)
                {
                    var livenessResult = await RunLiveness(liveness, options, httpContext);

                    return new[] { livenessResult };
                }
            }

            return Enumerable.Empty<LivenessResult>();
        }

        public void Track(LivenessResult responses)
        {
            if (_beatPulseContext.AllTrackers != null)
            {
                foreach (var tracker in _beatPulseContext.AllTrackers)
                {
                    tracker.Track(responses);
                }
            }
        }

        async Task<LivenessResult> RunLiveness(IBeatPulseLiveness liveness, BeatPulseOptions options, HttpContext httpContext)
        {
            _logger.LogInformation($"Executing liveness {liveness.Name}.");

            var livenessResult = new LivenessResult(liveness.Name, liveness.DefaultPath);

            livenessResult.StartCounter();

            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    var livenessTask = liveness.IsHealthy(httpContext, _environment.IsDevelopment(), cancellationTokenSource.Token);

                    if (await Task.WhenAny(livenessTask, Task.Delay(options.Timeout, cancellationTokenSource.Token)) == livenessTask)
                    {
                        // The liveness is executed successfully and get the results
                        var (message, healthy) = await livenessTask;
                        livenessResult.StopCounter(message, healthy);

                        _logger.LogInformation($"The liveness {liveness.Name} is executed.");
                    }
                    else
                    {
                        // The liveness is timeout ( from configured options)
                        _logger.LogWarning($"The liveness {liveness.Name} is timeout");

                        cancellationTokenSource.Cancel();
                        livenessResult.StopCounter(BeatPulseKeys.BEATPULSE_TIMEOUT_MESSAGE, false);
                    }
                }   
            }
            catch (Exception ex)
            {
                // The uri executed is now well formed, dns not found
                // or any unexpected errror 
                _logger.LogError(ex, $"The liveness {liveness.Name} is unhealthy.");

                var message = _environment.IsDevelopment()
                    ? ex.Message : string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, liveness.Name);

                livenessResult.StopCounter(message, false);
            }

            return livenessResult;
        }
    }
}
