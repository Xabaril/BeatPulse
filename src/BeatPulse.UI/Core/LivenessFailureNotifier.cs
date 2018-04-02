using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    public class LivenessFailureNotifier
        : ILivenessFailureNotifier
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LivenessFailureNotifier> _logger;

        public LivenessFailureNotifier(IConfiguration configuration, ILogger<LivenessFailureNotifier> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyFailure(string livenessName, string content)
        {
            const string jsonContentType = "application/json";
            const string webHookUriConfigurationKey = "BeatPulseUIWebHookNotifier";

            var webHook = _configuration[webHookUriConfigurationKey];

            if (webHook != null)
            {
                using (var httpClient = new HttpClient())
                {
                    var payload = new StringContent(content, Encoding.UTF8, jsonContentType);

                    try
                    {
                        var response = await httpClient.PostAsync(webHook, payload);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError($"The failure notification is not executed successfully. The error code is {response.StatusCode}.");
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError($"The failure notification is not executed successfully.",exception);
                    }
                }
            }
        }
    }
}
