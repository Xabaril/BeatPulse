using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.IdSvr
{
    public class IdSvrLiveness
        : IBeatPulseLiveness
    {
        const string IDSVR_DISCOVER_CONFIGURATION_SEGMENT = ".well-known/openid-configuration";

        private readonly Uri _idSvrUri;
        private readonly ILogger<IdSvrLiveness> _logger;

        public IdSvrLiveness(Uri idSvrUri,ILogger<IdSvrLiveness> logger = null)
        {
            _idSvrUri = idSvrUri ?? throw new ArgumentNullException(nameof(idSvrUri));
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(IdSvrLiveness)} is checking the IdSvr on {_idSvrUri}.");

                using (var httpClient = new HttpClient() { BaseAddress = _idSvrUri })
                {
                    var response = await httpClient.GetAsync(IDSVR_DISCOVER_CONFIGURATION_SEGMENT);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger?.LogWarning($"The {nameof(IdSvrLiveness)} check failed for server {_idSvrUri}.");

                        var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                            : $"Discover endpoint is not responding with 200 OK, the current status is {response.StatusCode} and the content { (await response.Content.ReadAsStringAsync())}";

                        return (message, false);
                    }

                    _logger?.LogInformation($"The {nameof(IdSvrLiveness)} check success.");

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(IdSvrLiveness)} check fail for IdSvr with the exception {ex.ToString()}.");

                var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
