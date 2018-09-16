using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse
{
    public class FtpLiveness : IHealthCheck
    {
        private readonly FtpLivenessOptions _options;

        public FtpLiveness(FtpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentException(nameof(options));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
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
                            return HealthCheckResult.Failed($"Error connecting to ftp host {item.host} the exit code eas {ftpResponse.StatusCode}");
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
