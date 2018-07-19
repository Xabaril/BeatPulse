using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Uris
{
    public class UriLiveness
        : IBeatPulseLiveness
    {
        private readonly IEnumerable<Uri> _uris;
        private readonly HttpMethod _httpMethod;

        public UriLiveness(IEnumerable<Uri> uris, HttpMethod httpMethod)
        {
            _uris = uris;
            _httpMethod = httpMethod;
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
            foreach (var item in _uris)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, false);
                }

                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var requestMessage = new HttpRequestMessage(_httpMethod, item);

                        var response = await httpClient.SendAsync(requestMessage);

                        if (!response.IsSuccessStatusCode)
                        {
                            var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                                : $"Discover endpoint is not responding with SuccessStatusCode, the current status is {response.StatusCode}.";

                            return (message, false);
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

            return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
        }
    }
}
