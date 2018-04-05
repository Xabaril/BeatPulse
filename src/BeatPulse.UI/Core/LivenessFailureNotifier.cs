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

            var webHookUriKey = $"{Globals.BEATPULSEUI_SECTION_SETTING_KEY}:{Globals.WEBHOOK_NOTIFICATION_SETTING_KEY}";

            var webHookConfigurationValue = _configuration[webHookUriKey];

            if (webHookConfigurationValue != null
                && Uri.TryCreate(webHookConfigurationValue, UriKind.Absolute, out Uri webHookUri))
            {
                using (var httpClient = new HttpClient())
                {
                    var payload = new StringContent(content, Encoding.UTF8, jsonContentType);

                    try
                    {
                        var response = await httpClient.PostAsync(webHookUri, payload);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError($"The failure notification is not executed successfully. The error code is {response.StatusCode}.");
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError($"The failure notification is not executed successfully.", exception);
                    }
                }
            }
            else
            {
                _logger.LogWarning($"The web hook notification uri is not stablished or is not an absolute Uri. Set the webhook uri value on {Globals.WEBHOOK_NOTIFICATION_SETTING_KEY} setting key.");
            }
        }
    }
}
