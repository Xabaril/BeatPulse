using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Network
{
    public class DnsResolveLiveness : IBeatPulseLiveness
    {
        private readonly DnsResolveOptions _options;
        private readonly ILogger<DnsResolveLiveness> _logger;

        public DnsResolveLiveness(DnsResolveOptions options, ILogger<DnsResolveLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options)); ;
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(DnsResolveLiveness)} is checking DNS entries.");

                foreach (var item in _options.ConfigureHosts.Values)
                {
                    var ipAddresses = await Dns.GetHostAddressesAsync(item.Host);

                    foreach (var ipAddress in ipAddresses)
                    {
                        if (!item.Resolutions.Contains(ipAddress.ToString()))
                        {
                            _logger?.LogWarning($"The {nameof(DnsResolveLiveness)} check fail for {ipAddress} was not resolved from host {item.Host}.");

                            return LivenessResult.UnHealthy("Ip Address {ipAddress} was not resolved from host {item.Host}");
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(DnsResolveLiveness)} check success.");

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(DnsResolveLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
