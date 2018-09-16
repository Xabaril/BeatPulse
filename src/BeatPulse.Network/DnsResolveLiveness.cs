using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Network
{
    public class DnsResolveLiveness : IHealthCheck
    {
        private readonly DnsResolveOptions _options;

        public DnsResolveLiveness(DnsResolveOptions options)
        {
            _options = options;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var item in _options.ConfigureHosts.Values)
                {
                    var ipAddresses = Dns.GetHostAddresses(item.Host)
                                         .Select(h => h.ToString());

                    foreach (var ipAddress in ipAddresses)
                    {
                        if (!item.Resolutions.Contains(ipAddress))
                        {
                            return await Task.FromResult(HealthCheckResult.Failed($"Ip Address {ipAddress} was not resolved from host {item.Host}"));
                        }
                    }
                }

                return await Task.FromResult(HealthCheckResult.Passed());
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
