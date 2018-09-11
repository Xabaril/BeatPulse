using BeatPulse.Core;
using BeatPulse.Network;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace BeatPulse
{
    public class SftpLiveness : IBeatPulseLiveness
    {
        private readonly SftpLivenessOptions _options;
        private readonly ILogger<SftpLiveness> _logger;

        public SftpLiveness(SftpLivenessOptions options, ILogger<SftpLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(SftpLiveness)} is checking SFTP connections.");

                foreach (var item in _options.ConfiguredHosts.Values)
                {
                    var connectionInfo = new ConnectionInfo(item.Host, item.UserName, item.AuthenticationMethods.ToArray());

                    using (var sftpClient = new SftpClient(connectionInfo))
                    {
                        sftpClient.Connect();

                        var connectionSuccess = sftpClient.IsConnected && sftpClient.ConnectionInfo.IsAuthenticated;

                        if (connectionSuccess)
                        {
                            if (item.FileCreationOptions.createFile)
                            {
                                using (var stream = new MemoryStream(new byte[] { 0x0 }, 0, 1))
                                {
                                    sftpClient.UploadFile(stream, item.FileCreationOptions.remoteFilePath);
                                }
                            }
                        }
                        else
                        {
                            _logger?.LogWarning($"The {nameof(SftpLiveness)} check fail for sftp host {item.Host}.");

                            return Task.FromResult(($"Connection with sftp host {item.Host}:{item.Port} failed", false));
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(SftpLiveness)} check success.");

                return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));

            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(SftpLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return Task.FromResult((message, false));
            }
        }
    }
}
