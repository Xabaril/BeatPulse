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

        public SmtpLiveness(SmtpLivenessOptions options, ILogger<SmtpLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(SmtpLiveness)} is checking SMTP connections.");

                using (var smtpConnection = new SmtpConnection(_options))
                {
                    if (await smtpConnection.ConnectAsync())
                    {
                        if (_options.AccountOptions.Login)
                        {
                            var (user, password) = _options.AccountOptions.Account;

                            if (!await smtpConnection.AuthenticateAsync(user, password))
                            {
                                _logger?.LogWarning($"The {nameof(SmtpLiveness)} check fail with invalid login to smtp server {_options.Host}:{_options.Port} with configured credentials.");

                                return LivenessResult.UnHealthy($"Error login to smtp server{_options.Host}:{_options.Port} with configured credentials");
                            }
                        }

                        _logger?.LogInformation($"The {nameof(SmtpLiveness)} check success.");

                        return LivenessResult.Healthy();
                    }
                    else
                    {
                        _logger?.LogWarning($"The {nameof(SmtpLiveness)} check fail for connecting to smtp server {_options.Host}:{_options.Port} - SSL : {_options.ConnectionType}.");

                        return LivenessResult.UnHealthy($"Could not connect to smtp server {_options.Host}:{_options.Port} - SSL : {_options.ConnectionType}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(SmtpLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex, showDetailedErrors: context.ShowDetailedErrors);
            }
        }
    }
}
