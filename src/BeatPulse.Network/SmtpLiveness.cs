using System;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Network.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Network
{
    public class SmtpLiveness : IHealthCheck
    {
        private readonly SmtpLivenessOptions _options;

        public SmtpLiveness(SmtpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var smtpConnection = new SmtpConnection(_options))
                {
                    if (await smtpConnection.ConnectAsync())
                    {
                        if (_options.AccountOptions.login)
                        {
                            var (user, password) = _options.AccountOptions.account;

                            if (!await smtpConnection.AuthenticateAsync(user, password))
                            {
                                return HealthCheckResult.Failed($"Error login to smtp server{_options.Host}:{_options.Port} with configured credentials");
                            }
                        }

                        return HealthCheckResult.Passed();
                    }
                    else
                    {
                        return HealthCheckResult.Failed($"Could not connect to smtp server {_options.Host}:{_options.Port} - SSL : {_options.ConnectionType}");
                    }
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
