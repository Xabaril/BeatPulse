using BeatPulse.UI.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    class LivenessFailureNotifier
        : ILivenessFailureNotifier
    {
        private readonly ILogger<LivenessFailureNotifier> _logger;
        private readonly BeatPulseSettings _settings;

        public LivenessFailureNotifier(IOptions<BeatPulseSettings> settings, ILogger<LivenessFailureNotifier> logger)
        {
            _settings = settings.Value ?? new BeatPulseSettings();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyFailure(string livenessName, string content)
        {
            if (_settings.WebHookNotificationUri != null
                && Uri.TryCreate(_settings.WebHookNotificationUri, UriKind.Absolute, out Uri webHookUri))
            {
                using (var httpClient = new HttpClient())
                {
                    var payload = new StringContent(content, Encoding.UTF8, Globals.DEFAULT_RESPONSE_CONTENT_TYPE);

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
