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

        public async Task<IEnumerable<LivenessResult>> IsHealthy(string path, HttpContext httpContext,CancellationToken cancellationToken)
        {
            _logger.LogInformation($"BeatPulse is checking health on {path}");

            if (String.IsNullOrEmpty(path))
            {
                var livenessResults = new List<LivenessResult>();

                foreach (var liveness in _beatPulseContext.AllLiveness)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var healthCheckResult = await RunLiveness(liveness, httpContext);

                    livenessResults.Add(healthCheckResult);

                    if (!healthCheckResult.IsHealthy)
                    {
                        //break when first check is not healthy

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
                    var livenessResult = await RunLiveness(liveness, httpContext);

                    return new[] { livenessResult };
                }
            }

            return Enumerable.Empty<LivenessResult>();
        }

        async Task<LivenessResult> RunLiveness(IBeatPulseLiveness beatPulseCheck, HttpContext httpContext)
        {
            var livenessResult = new LivenessResult(beatPulseCheck.Name,beatPulseCheck.DefaultPath);

            livenessResult.StartCounter();

            var (message, healthy) = await beatPulseCheck.IsHealthy(httpContext, _environment.IsDevelopment());

            livenessResult.StopCounter(message, healthy);

            return livenessResult;
        }
    }
}
