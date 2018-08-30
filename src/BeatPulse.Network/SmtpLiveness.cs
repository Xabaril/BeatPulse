using BeatPulse.Core;
using BeatPulse.Network.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Network
{
    public class SmtpLiveness : IBeatPulseLiveness
    {
        private readonly SmtpLivenessOptions _options;


        public SmtpLiveness(SmtpLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                using(var smtpConnection = new SmtpConnection(_options))
                {
                    if(await smtpConnection.ConnectAsync())
                    {
                        if (_options.AccountOptions.login)
                        {
                            var (user, password) = _options.AccountOptions.account;
                            var authResult = await smtpConnection.AuthenticateAsync(user, password);
                            if(!authResult)
                            {
                                return ($"Error login to smtp server{_options.Host}:{_options.Port} with configured credentials", false);
                            }
                        }

                        return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                    }
                    else
                    {
                        return ($"Could not connect to smtp server {_options.Host}:{_options.Port} - SSL : {_options.ConnectionType}", false);
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
    }
}
