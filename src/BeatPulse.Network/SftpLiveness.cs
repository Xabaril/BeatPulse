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

        public Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
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

                            return Task.FromResult(
                                LivenessResult.UnHealthy($"Connection with sftp host {item.Host}:{item.Port} failed"));
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(SftpLiveness)} check success.");

                return Task.FromResult(
                    LivenessResult.Healthy());

            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(SftpLiveness)} check fail with the exception {ex.ToString()}.");

                return Task.FromResult(
                    LivenessResult.UnHealthy(ex, showDetailedErrors: context.ShowDetailedErrors));
            }
        }
    }
}
