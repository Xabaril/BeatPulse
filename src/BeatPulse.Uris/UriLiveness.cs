using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Uris
{
    public class UriLiveness : IHealthCheck
    {
        private readonly UriLivenessOptions _options;

        public UriLiveness(UriLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var defaultHttpMethod = _options.HttpMethod;
            var defaultCodes = _options.ExpectedHttpCodes;
            var idx = 0;

            foreach (var item in _options.UrisOptions)
            {
                var method = item.HttpMethod ?? defaultHttpMethod;
                var expectedCodes = item.ExpectedHttpCodes ?? defaultCodes;

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
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
                            var message =  $"Discover endpoint #{idx} is not responding with code in {expectedCodes.Min}...{expectedCodes.Max} range, the current status is {response.StatusCode}.";
                            return HealthCheckResult.Failed(description: message);
                        }

                        ++idx;
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Failed(exception: ex);
                }
            }

            return HealthCheckResult.Passed();
        }
    }
}
