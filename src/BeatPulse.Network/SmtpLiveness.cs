using BeatPulse.Core;
using BeatPulse.Network.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Network
{
    public class SmtpLiveness : IBeatPulseLiveness
    {
        private readonly SmtpLivenessOptions _options;
        private readonly ILogger<SmtpLiveness> _logger;

        public SmtpLiveness(SmtpLivenessOptions options,ILogger<SmtpLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(SmtpLiveness)} is checking SMTP connections.");

                using (var smtpConnection = new SmtpConnection(_options))
                {
                    if(await smtpConnection.ConnectAsync())
                    {
                        if (_options.AccountOptions.login)
                        {
                            var (user, password) = _options.AccountOptions.account;

                            if(! await smtpConnection.AuthenticateAsync(user, password))
                            {
                                _logger?.LogWarning($"The {nameof(SmtpLiveness)} check fail with invalid login to smtp server{_options.Host}:{_options.Port} with configured credentials.");

                                return ($"Error login to smtp server{_options.Host}:{_options.Port} with configured credentials", false);
                            }
                        }

                        _logger?.LogInformation($"The {nameof(SmtpLiveness)} check success.");

                        return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                    }
                    else
                    {
                        _logger?.LogWarning($"The {nameof(SmtpLiveness)} check fail for connecting to smtp server {_options.Host}:{_options.Port} - SSL : {_options.ConnectionType}.");

                        return ($"Could not connect to smtp server {_options.Host}:{_options.Port} - SSL : {_options.ConnectionType}", false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(SmtpLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                  : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
