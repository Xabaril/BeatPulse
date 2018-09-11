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
        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(TcpLiveness)} is checking hosts.");

                foreach (var item in _options.ConfiguredHosts)
                {
                    using (var tcpClient = new TcpClient())
                    {
                        await tcpClient.ConnectAsync(item.host, item.port);

                        if (!tcpClient.Connected)
                        {
                            _logger?.LogWarning($"The {nameof(TcpLiveness)} check failed for host {item.host} and port {item.port}.");

                            return ($"Connection to host {item.host}:{item.port} failed", false);
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(TcpLiveness)} check success.");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(TcpLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
