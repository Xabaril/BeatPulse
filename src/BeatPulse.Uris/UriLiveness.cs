using BeatPulse.Core;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Uris
{
    public class UriLiveness
        : IBeatPulseLiveness
    {
        private readonly UriLivenessOptions _options;

        public UriLiveness(UriLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            var defaultHttpMethod = _options.HttpMethod;
            var defaultCodes = _options.ExpectedHttpCodes;
            var idx = 0;

            try
            {
                foreach (var item in _options.UrisOptions)
                {
                    var method = item.HttpMethod ?? defaultHttpMethod;
                    var expectedCodes = item.ExpectedHttpCodes ?? defaultCodes;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, false);
                    }

                    using (var httpClient = new HttpClient())
                    {
                        var requestMessage = new HttpRequestMessage(method, item.Uri);

                        foreach (var header in item.Headers)
                        {
                            requestMessage.Headers.Add(header.Name, header.Value);
                        }

                        var response = await httpClient.SendAsync(requestMessage);

                        if (!((int)response.StatusCode >= expectedCodes.Min && (int)response.StatusCode <= expectedCodes.Max))
                        {
                            var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                                : $"Discover endpoint #{idx} is not responding with code in {expectedCodes.Min}...{expectedCodes.Max} range, the current status is {response.StatusCode}.";

                            return (message, false);
                        }

                        ++idx;
                    }

                }
            }
            catch (Exception ex)
            {
                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }

            return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
        }
    }
}
