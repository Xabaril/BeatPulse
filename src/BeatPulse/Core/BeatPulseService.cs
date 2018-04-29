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

        public async Task<IEnumerable<LivenessResult>> IsHealthy(string path, BeatPulseOptions options, HttpContext httpContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"BeatPulse is checking health on [BeatPulsePath]/{path}");

            if (String.IsNullOrEmpty(path))
            {
                var livenessResults = new List<LivenessResult>();

                foreach (var liveness in _beatPulseContext.AllLiveness)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("BeatPulse execution is cancelled");

                        break;
                    }

                    var healthCheckResult = await RunLiveness(liveness, httpContext, cancellationToken);

                    livenessResults.Add(healthCheckResult);

                    if (!healthCheckResult.IsHealthy && !options.DetailedOutput)
                    {
                        //break when first check is not healthy

                        var warningMessage = cancellationToken.IsCancellationRequested
                            ? $"Liveness {liveness.Name} was cancelled on timeout"
                            : $"Liveness {liveness.Name} is not healthy";

                        _logger.LogWarning(warningMessage);

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
                    var livenessResult = await RunLiveness(liveness, httpContext, cancellationToken);

                    return new[] { livenessResult };
                }
            }

            return Enumerable.Empty<LivenessResult>();
        }

        async Task<LivenessResult> RunLiveness(IBeatPulseLiveness liveness, HttpContext httpContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executing liveness {liveness.Name}.");

            var livenessResult = new LivenessResult(liveness.Name, liveness.DefaultPath);

            livenessResult.StartCounter();

            try
            {
                var (message, healthy) = await liveness.IsHealthy(httpContext, _environment.IsDevelopment(), cancellationToken);

                livenessResult.StopCounter(message, healthy);

                _logger.LogInformation($"The liveness {liveness.Name} is executed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"The liveness {liveness.Name} is unhealthy.");

                var message = _environment.IsDevelopment()
                    ? ex.Message : string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, liveness.Name);

                livenessResult.StopCounter(message, false);
            }

            return livenessResult;
        }
    }
}
