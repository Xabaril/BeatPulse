﻿using BeatPulse.Core;
using BeatPulse.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse
{
    public class FtpLiveness : IBeatPulseLiveness
    {
        private readonly FtpLivenessOptions _options;
        private readonly ILogger<FtpLiveness> _logger;

        public FtpLiveness(FtpLivenessOptions options, ILogger<FtpLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentException(nameof(options));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(FtpLiveness)} is checking FTP connections.");

                foreach (var item in _options.Hosts.Values)
                {
                    var ftpRequest = CreateFtpWebRequest(item.host, item.createFile, item.credentials);

                    using (var ftpResponse = (FtpWebResponse)await ftpRequest.GetResponseAsync())
                    {
                        if (ftpResponse.StatusCode != FtpStatusCode.PathnameCreated
                            && ftpResponse.StatusCode != FtpStatusCode.ClosingData)
                        {
                            _logger?.LogWarning($"The {nameof(FtpLiveness)} check fail for ftp host {item.host} with exit code {ftpResponse.StatusCode}.");

                            LivenessResult.UnHealthy($"Error connecting to ftp host {item.host} with exit code {ftpResponse.StatusCode}");
                        }
                    }
                }

                _logger?.LogInformation($"The {nameof(FtpLiveness)} check success.");

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(FtpLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
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
