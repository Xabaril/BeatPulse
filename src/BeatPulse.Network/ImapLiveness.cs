using BeatPulse.Core;
using BeatPulse.Network.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Network
{
    public class ImapLiveness : IBeatPulseLiveness
    {
        private readonly ImapLivenessOptions _options;

        private ImapConnection _imapConnection = null;

        public ImapLiveness(ImapLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(_options.Host)) throw new ArgumentNullException(nameof(_options.Host));
            if (_options.Port == default) throw new ArgumentNullException(nameof(_options.Port));

        }
        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
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
                    return ($"Connection to server {_options.Host} has failed - SSL Enabled : {_options.ConnectionType}", false);
                }

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);

            }
            catch (Exception ex)
            {
                var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                   : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
            finally
            {
                _imapConnection.Dispose();
            }
        }

        private async Task<(string, bool)> ExecuteAuthenticatedUserActions()
        {
            var (user, password) = _options.AccountOptions.account;

            if (await _imapConnection.AuthenticateAsync(user, password))
            {
                if (_options.FolderOptions.checkFolder)
                {
                    return await CheckConfiguredImapFolder();
                }

                return (string.Empty, true);
            }
            else
            {
                return ($"Login on server {_options.Host} failed with configured user", false);
            }


            async Task<(string, bool)> CheckConfiguredImapFolder()
            {
                return await _imapConnection.SelectFolder(_options.FolderOptions.folderName) ?
                    (string.Empty, true) :
                    ($"Folder {_options.FolderOptions.folderName} check failed", false);
            }
        }
    }
}
