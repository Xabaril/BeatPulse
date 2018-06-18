using BeatPulse.Core;
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

        public IdSvrLiveness(Uri idSvrUri)
        {
            _idSvrUri = idSvrUri ?? throw new ArgumentNullException(nameof(idSvrUri));
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var httpClient = new HttpClient() { BaseAddress = _idSvrUri })
                {
                    var response = await httpClient.GetAsync(IDSVR_DISCOVER_CONFIGURATION_SEGMENT);

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                            : $"Discover endpoint is not responding with 200 OK, the current status is {response.StatusCode} and the content { (await response.Content.ReadAsStringAsync())}";

                        return (message, false);
                    }

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
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
