using BeatPulse.Core;
using BeatPulse.Network;
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

        public SftpLiveness(SftpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
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
                            return Task.FromResult(($"Connection with sftp host {item.Host}:{item.Port} failed", false));
                        }
                    }
                }

                return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));

            }
            catch (Exception ex)
            {
                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return Task.FromResult((message, false));
            }

        }
    }
}
