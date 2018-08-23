using BeatPulse.Core;
using BeatPulse.Network;
using Microsoft.AspNetCore.Http;
using Renci.SshNet;
using System;
using System.Threading;
using System.Threading.Tasks;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace BeatPulse
{
    public class SftpLiveness : IBeatPulseLiveness
    {
        private readonly SftpLivenessOptions _options;

        public SftpLiveness(SftpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext,
            CancellationToken cancellationToken = default)
        {
            foreach (var item in _options.ConfiguredHosts.Values)
            {
                try
                {                    
                    var connectionInfo = new ConnectionInfo(item.Host, item.UserName, item.AuthenticationMethods.ToArray());

                    using (var sftpClient = new SftpClient(connectionInfo))
                    {
                        sftpClient.Connect();
                        
                        var connectionSuccess = sftpClient.IsConnected && sftpClient.ConnectionInfo.IsAuthenticated;
                        
                        if (!connectionSuccess)
                        {
                            return Task.FromResult(($"Connection with sftp host {item.Host}:{item.Port} failed", false));
                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                    return Task.FromResult((message, false));
                }

            }

            return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
        }
        
    }
}
