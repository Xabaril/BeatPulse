using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
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

        public DnsResolveLiveness(DnsResolveOptions options)
        {
            _options = options;
        }
        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
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
                            return await Task.FromResult(($"Ip Address {ipAddress} was not resolved from host {item.Host}", false));
                        }
                    }                   
                }

                return await Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
            }
            catch (Exception ex)
            {
                var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return await Task.FromResult((message, false));
            }
        }
    }
}
