using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    class BeatPulseService
        : IBeatPulseService
    {
        private readonly BeatPulseChecks _checks;
        private readonly ILogger<BeatPulseService> _logger;

        public BeatPulseService(BeatPulseChecks checks, ILogger<BeatPulseService> logger)
        {
            _checks = checks ?? throw new ArgumentNullException(nameof(checks));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<bool> IsHealthy(string path,HttpContext context)
        {
            return Task.FromResult(true);
        }
    }
}
