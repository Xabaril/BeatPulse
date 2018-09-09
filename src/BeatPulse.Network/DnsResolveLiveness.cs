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
        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(DnsResolveLiveness)} is checking DNS entries.");

                foreach (var item in _options.ConfigureHosts.Values)
                {
                    var ipAddresses = Dns.GetHostAddresses(item.Host)
                                         .Select(h => h.ToString());

                    foreach (var ipAddress in ipAddresses)
                    {
                        if (!item.Resolutions.Contains(ipAddress))
                        {
                            _logger?.LogWarning($"The {nameof(DnsResolveLiveness)} check fail for {ipAddress} was not resolved from host {item.Host}.");

                            return await Task.FromResult(($"Ip Address {ipAddress} was not resolved from host {item.Host}", false));
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(DnsResolveLiveness)} check success.");

                return await Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(DnsResolveLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return await Task.FromResult((message, false));
            }
        }
    }
}
