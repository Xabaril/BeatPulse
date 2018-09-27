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

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            var configuredHosts = _options.ConfiguredHosts.Values;

            try
            {
                _logger?.LogInformation($"{nameof(PingLiveness)} is checking hosts.");

                foreach (var (host, timeout) in configuredHosts)
                {
                    using (var ping = new Ping())
                    {
                        var pingReply = await ping.SendPingAsync(host, timeout);

                        if (pingReply.Status != IPStatus.Success)
                        {
                            _logger?.LogWarning($"The {nameof(PingLiveness)} check failed for host {host} is failed with status reply:{pingReply.Status}.");

                            return LivenessResult.UnHealthy($"Ping check for host {host} is failed with status reply:{pingReply.Status}");
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(PingLiveness)} check success.");

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(PingLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
