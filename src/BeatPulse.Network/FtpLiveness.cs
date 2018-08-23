using BeatPulse.Core;
using BeatPulse.Network;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse
{
    public class FtpLiveness : IBeatPulseLiveness
    {
        private readonly FtpLivenessOptions _options;

        public FtpLiveness(FtpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentException(nameof(options));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext,
            CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var item in _options.Hosts.Values)
                {
                    var ftpRequest = CreateFtpWebRequest(item.host, item.createFile, item.credentials);

                    using (var ftpResponse = (FtpWebResponse)await ftpRequest.GetResponseAsync())
                    {
                        if (ftpResponse.StatusCode != FtpStatusCode.PathnameCreated
                            && ftpResponse.StatusCode != FtpStatusCode.ClosingData)
                        {
                            return ($"Error connecting to ftp host {item.host} the exit code eas {ftpResponse.StatusCode}", false);
                        }
                    }
                }

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }

        WebRequest CreateFtpWebRequest(string host, bool createFile = false, NetworkCredential credentials = null)
        {
            FtpWebRequest ftpRequest;

            if (createFile)
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create($"{host}/beatpulse");

                if (credentials != null)
                {
                    ftpRequest.Credentials = credentials;
                }

                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                using (var stream = ftpRequest.GetRequestStream())
                {
                    stream.Write(new byte[] { 0x0 }, 0, 1);
                }
            }
            else
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(host);

                if (credentials != null)
                {
                    ftpRequest.Credentials = credentials;
                }

                ftpRequest.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
            }

            return ftpRequest;
        }
    }
}
