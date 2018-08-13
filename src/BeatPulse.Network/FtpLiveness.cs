using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Network;

namespace BeatPulse
{
    public class FtpLiveness : IBeatPulseLiveness
    {
        private readonly FtpLivenessOptions _options;

        public FtpLiveness(FtpLivenessOptions options)
        {
            _options = options;
        }
        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext,
            CancellationToken cancellationToken = default)
        {
            foreach (var item in _options.ConfiguredHosts.Values)
            {
                try
                {

                    var ftpRequest = (FtpWebRequest)WebRequest.Create(item.host);
                    ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;

                    if (item.credentials != null)
                    {
                        ftpRequest.Credentials = item.credentials;
                    }

                    using (var ftpResponse = (FtpWebResponse)await ftpRequest.GetResponseAsync())
                    {
                        if (ftpResponse.StatusCode != FtpStatusCode.CommandOK)
                        {
                            return ($"Error connecting to ftp host {item.host} the exit code eas {ftpResponse.StatusCode}", false);
                        }
                    }

                }
                catch (Exception ex)
                {
                    var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                        : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                    return (message, false);
                }
            }

            return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);

        }
    }
}
