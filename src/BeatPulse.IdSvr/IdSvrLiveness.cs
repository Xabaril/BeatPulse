using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.IdSvr
{
    public class IdSvrLiveness : IHealthCheck
    {
        const string IDSVR_DISCOVER_CONFIGURATION_SEGMENT = ".well-known/openid-configuration";

        private readonly Uri _idSvrUri;

        public IdSvrLiveness(Uri idSvrUri)
        {
            _idSvrUri = idSvrUri ?? throw new ArgumentNullException(nameof(idSvrUri));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var httpClient = new HttpClient() { BaseAddress = _idSvrUri })
                {
                    var response = await httpClient.GetAsync(IDSVR_DISCOVER_CONFIGURATION_SEGMENT);

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = $"Discover endpoint is not responding with 200 OK, the current status is {response.StatusCode} and the content { (await response.Content.ReadAsStringAsync())}";

                        return HealthCheckResult.Failed(message);
                    }

                    return HealthCheckResult.Passed();
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
