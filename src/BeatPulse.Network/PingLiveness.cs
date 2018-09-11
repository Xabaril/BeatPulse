using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Network
{
    public class PingLiveness : IBeatPulseLiveness
    {
        private readonly PingLivenessOptions _options;
        private readonly ILogger<PingLiveness> _logger;

        public PingLiveness(PingLivenessOptions options, ILogger<PingLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            var configuredHosts = _options.ConfiguredHosts.Values;

            try
            {
                _logger?.LogInformation($"{nameof(PingLiveness)} is checking hosts.");

                foreach (var item in configuredHosts)
                {
                    using (var ping = new Ping())
                    {
                        var pingReply = await ping.SendPingAsync(item.Host, item.TimeOut);

                        if (pingReply.Status != IPStatus.Success)
                        {
                            _logger?.LogWarning($"The {nameof(PingLiveness)} check failed for host {item.Host} is failed with status reply:{pingReply.Status}.");

                            return ($"Ping check for host {item.Host} is failed with status reply:{pingReply.Status}", false);
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(PingLiveness)} check success.");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(PingLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                   : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
