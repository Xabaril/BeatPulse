using BeatPulse.Core;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UriLiveness> _logger;

        public UriLiveness(UriLivenessOptions options,ILogger<UriLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            var defaultHttpMethod = _options.HttpMethod;
            var defaultCodes = _options.ExpectedHttpCodes;
            var idx = 0;

            try
            {
                _logger?.LogInformation($"{nameof(UriLiveness)} is checking configured uri's.");

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
                            _logger?.LogWarning($"The {nameof(UriLiveness)} check fail for uri {item.Uri}.");

                            var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                                : $"Discover endpoint #{idx} is not responding with code in {expectedCodes.Min}...{expectedCodes.Max} range, the current status is {response.StatusCode}.";

                            return (message, false);
                        }

                        ++idx;
                    }
                }

                _logger?.LogDebug($"The {nameof(UriLiveness)} check success.");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(UriLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.ShowDetailedErrors ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
