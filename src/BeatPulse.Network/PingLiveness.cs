using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Network
{
    public class PingLiveness : IHealthCheck
    {
        private readonly PingLivenessOptions _options;

        public PingLiveness(PingLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var configuredHosts = _options.ConfiguredHosts.Values;

            foreach (var item in configuredHosts)
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        var pingReply = await ping.SendPingAsync(item.Host, item.TimeOut);

                        if (pingReply.Status != IPStatus.Success)
                        {
                            return HealthCheckResult.Failed($"Ping check for host {item.Host} is failed with status reply:{pingReply.Status}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Failed(exception: ex);
                }
            }

            return HealthCheckResult.Passed();
        }
    }
}
