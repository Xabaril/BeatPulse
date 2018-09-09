using BeatPulse.Core;
using BeatPulse.Network.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Network
{
    public class ImapLiveness : IBeatPulseLiveness
    {
        private readonly ImapLivenessOptions _options;
        private readonly ILogger<ImapLiveness> _logger;

        private ImapConnection _imapConnection = null;

        public ImapLiveness(ImapLivenessOptions options, ILogger<ImapLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;

            if (string.IsNullOrEmpty(_options.Host))
            {
                throw new ArgumentNullException(nameof(_options.Host));
            }

            if (_options.Port == default)
            {
                throw new ArgumentNullException(nameof(_options.Port));
            }

        }
        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(ImapLiveness)} is checking IMAP entries.");

                _imapConnection = new ImapConnection(_options);

                if (await _imapConnection.ConnectAsync())
                {
                    if (_options.AccountOptions.Login)
                    {
                        return await ExecuteAuthenticatedUserActions();
                    }
                }
                else
                {
                    _logger?.LogWarning($"{nameof(ImapLiveness)} fail connect to server {_options.Host}- SSL Enabled : {_options.ConnectionType}.");

                    return ($"Connection to server {_options.Host} has failed - SSL Enabled : {_options.ConnectionType}", false);
                }

                _logger?.LogInformation($"The {nameof(ImapLiveness)} check success.");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);

            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(ImapLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
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
            var (User, Password) = _options.AccountOptions.Account;

            if (await _imapConnection.AuthenticateAsync(User, Password))
            {
                if (_options.FolderOptions.CheckFolder 
                    && ! await _imapConnection.SelectFolder(_options.FolderOptions.FolderName))
                {
                    _logger?.LogWarning($"{nameof(ImapLiveness)} fail connect to server {_options.Host}- SSL Enabled : {_options.ConnectionType} and open folder {_options.FolderOptions.FolderName}.");

                    return ($"Folder {_options.FolderOptions.FolderName} check failed.", false);
                }
                
                _logger?.LogInformation($"The {nameof(ImapLiveness)} check success.");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            else
            {
                _logger?.LogWarning($"{nameof(ImapLiveness)} fail connect to server {_options.Host}- SSL Enabled : {_options.ConnectionType}.");

                return ($"Login on server {_options.Host} failed with configured user", false);
            }
        }
    }
}
