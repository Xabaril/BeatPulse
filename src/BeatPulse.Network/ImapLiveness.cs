using System;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Network.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Network
{
    public class ImapLiveness : IHealthCheck
    {
        private readonly ImapLivenessOptions _options;

        private ImapConnection _imapConnection = null;

        public ImapLiveness(ImapLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(_options.Host))
            {
                throw new ArgumentNullException(nameof(_options.Host));
            }

            if (_options.Port == default)
            {
                throw new ArgumentNullException(nameof(_options.Port));
            }
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _imapConnection = new ImapConnection(_options);

                if (await _imapConnection.ConnectAsync())
                {
                    if (_options.AccountOptions.login)
                    {
                        return await ExecuteAuthenticatedUserActions();
                    }
                }
                else
                {
                    return HealthCheckResult.Failed($"Connection to server {_options.Host} has failed - SSL Enabled : {_options.ConnectionType}");
                }

                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
            finally
            {
                _imapConnection.Dispose();
            }
        }

        private async Task<HealthCheckResult> ExecuteAuthenticatedUserActions()
        {
            var (user, password) = _options.AccountOptions.account;

            if (await _imapConnection.AuthenticateAsync(user, password))
            {
                if (_options.FolderOptions.checkFolder)
                {
                    return await CheckConfiguredImapFolder();
                }

                return HealthCheckResult.Passed();
            }
            else
            {
                return HealthCheckResult.Failed($"Login on server {_options.Host} failed with configured user");
            }


            async Task<HealthCheckResult> CheckConfiguredImapFolder()
            {
                return await _imapConnection.SelectFolder(_options.FolderOptions.folderName) ?
                    HealthCheckResult.Passed() :
                    HealthCheckResult.Failed($"Folder {_options.FolderOptions.folderName} check failed");
            }
        }
    }
}
