using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Core;
using BeatPulse.System.Extensions;
using Microsoft.AspNetCore.Http;

namespace BeatPulse.System
{
    public class PingLiveness : IBeatPulseLiveness
    {
        private readonly PingLivenessOptions _options;

        public PingLiveness(PingLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {

            foreach (var registeredHost in _options.RegisteredHosts)
            {
                try
                {
                    var (host, timeout) = registeredHost.Value;

                    using (var ping = new Ping())
                    {
                        var pingReply = await ping.SendPingAsync(host, timeout);

                        if (pingReply.Status != IPStatus.Success)
                        {
                            var reason = pingReply.Status == IPStatus.TimedOut ? "timed out" : "failed";
                            return livenessContext.CreateErrorResponse($"Ping check for host {host} {reason}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var response = livenessContext.CreateErrorResponse($"Exception {ex.GetType().Name} with message (' {registeredHost.Value.Host} - {ex.Message}')");
                    return response;
                }
            }

            return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
        }
    }
}
