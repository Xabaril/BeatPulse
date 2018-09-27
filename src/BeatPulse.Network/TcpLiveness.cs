using BeatPulse.Core;
using BeatPulse.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse
{
    public class TcpLiveness : IBeatPulseLiveness
    {
        private readonly TcpLivenessOptions _options;
        private readonly ILogger<TcpLiveness> _logger;

        public TcpLiveness(TcpLivenessOptions options, ILogger<TcpLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }
        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(TcpLiveness)} is checking hosts.");

                foreach (var (host, port) in _options.ConfiguredHosts)
                {
                    using (var tcpClient = new TcpClient())
                    {
                        await tcpClient.ConnectAsync(host, port);

                        if (!tcpClient.Connected)
                        {
                            _logger?.LogWarning($"The {nameof(TcpLiveness)} check failed for host {host} and port {port}.");

                            return LivenessResult.UnHealthy($"Connection to host {host}:{port} failed");
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(TcpLiveness)} check success.");

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(TcpLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
