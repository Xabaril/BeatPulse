using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Network;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse
{
    public class TcpLiveness : IHealthCheck
    {
        private readonly TcpLivenessOptions _options;

        public TcpLiveness(TcpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var item in _options.ConfiguredHosts)
                {
                    using (var tcpClient = new TcpClient())
                    {
                        await tcpClient.ConnectAsync(item.host, item.port);

                        if (!tcpClient.Connected)
                        {
                            return HealthCheckResult.Failed($"Connection to host {item.host}:{item.port} failed");
                        }
                    }
                }

                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
