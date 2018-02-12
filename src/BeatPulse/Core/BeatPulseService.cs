using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public class BeatPulseService
        : IBeatPulseService
    {
        private readonly BeatPulseContext _context;
        private readonly ILogger<BeatPulseService> _logger;

        public BeatPulseService(BeatPulseContext context, ILogger<BeatPulseService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsHealthy(string path,HttpContext context)
        {
            _logger.LogInformation($"BeatPulse is checking health on {path}");

            if (String.IsNullOrEmpty(path))
            {
                foreach (var item in _context.All)
                {
                    if (! await item.IsHealthy(context))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                var checker = _context.Find(path);

                if (checker != null)
                {
                    return await checker.IsHealthy(context);
                }
            }

            return true;
        }
    }
}
