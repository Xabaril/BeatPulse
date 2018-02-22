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
        private readonly BeatPulseContext _context;
        private readonly ILogger<BeatPulseService> _logger;

        public BeatPulseService(BeatPulseContext context, ILogger<BeatPulseService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<HealthCheckMessage>> IsHealthy(string path, HttpContext context)
        {
            _logger.LogInformation($"BeatPulse is checking health on {path}");

            if (String.IsNullOrEmpty(path))
            {
                var messages = new List<HealthCheckMessage>(_context.ChecksCount);

                foreach (var item in _context.All)
                {
                    var message = new HealthCheckMessage(item.HealthCheckName);

                    if (item.Options.IncludeInOutput)
                    {
                        messages.Add(message);
                    }

                    message.StartCounter();

                    var (itemMessage, healthy) = await item.IsHealthy(context);

                    message.StopCounter(itemMessage, healthy);

                    if (!healthy)
                    {
                        return messages;
                    }
                }

                return messages;
            }
            else
            {
                var checker = _context.Find(path);

                if (checker != null)
                {
                    var message = new HealthCheckMessage(checker.HealthCheckName);
                    message.StartCounter();
                    var (checkerMessage, healthy) = await checker.IsHealthy(context);
                    message.StopCounter(checkerMessage, healthy);

                    return new[] { message };
                }
            }

            return Enumerable.Empty<HealthCheckMessage>();
        }
    }
}
