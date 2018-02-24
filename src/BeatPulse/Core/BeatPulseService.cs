using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<HealthCheckResult>> IsHealthy(string path, HttpContext httpContext)
        {
            _logger.LogInformation($"BeatPulse is checking health on {path}");

            if (String.IsNullOrEmpty(path))
            {
                var heathCheckResults = new List<HealthCheckResult>(_beatPulseContext.AllBeatPulseHealthChecks.Count());

                foreach (var healtCheck in _beatPulseContext.AllBeatPulseHealthChecks)
                {
                    var healthCheckResult = await RunBeatPulseCheck(healtCheck, httpContext);

                    heathCheckResults.Add(healthCheckResult);

                    if (!healthCheckResult.IsHealthy)
                    {
                        //break when first check is not healthy

                        return heathCheckResults;
                    }
                }

                return heathCheckResults;
            }
            else
            {
                var healtCheck = _beatPulseContext.FindBeatPulseHealthCheck(path);

                if (healtCheck != null)
                {
                    var healthCheckResult = await RunBeatPulseCheck(healtCheck, httpContext);

                    return new[] { healthCheckResult };
                }
            }

            return Enumerable.Empty<HealthCheckResult>();
        }

        async Task<HealthCheckResult> RunBeatPulseCheck(IBeatPulseHealthCheck beatPulseCheck, HttpContext httpContext)
        {
            var healthCheckMessage = new HealthCheckResult(beatPulseCheck.HealthCheckName);

            healthCheckMessage.StartCounter();

            var (message, healthy) = await beatPulseCheck.IsHealthy(httpContext, _environment.IsDevelopment());

            healthCheckMessage.StopCounter(message, healthy);

            return healthCheckMessage;
        }
    }
}
