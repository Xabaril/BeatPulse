using BeatPulse.Core;
using BeatPulse.Network;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse
{
    public class TcpLiveness : IBeatPulseLiveness
    {
        private readonly TcpLivenessOptions _options;

        public TcpLiveness(TcpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
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
                            return ($"Connection to host {item.host}:{item.port} failed", false);
                        }
                    }
                }

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }            
        }
    }
}
