using BeatPulse.UI.Configuration;
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
            foreach (var webHook in _settings.Webhooks)
            {
                if (webHook.Uri == null || !Uri.TryCreate(webHook.Uri, UriKind.Absolute, out Uri webHookUri))
                {
                    _logger.LogWarning($"The web hook notification uri is not stablished or is not an absolute Uri ({webHook.Name}). Set the webhook uri value on BeatPulse setttings.");

                    continue;
                }

                using (var httpClient = new HttpClient())
                {
                    var targetWebHook = new WebHookNotification()
                    {
                        Name = webHook.Name,
                        Uri = webHook.Uri,
                        Payload = webHook.Payload
                    };

                    targetWebHook.Payload = targetWebHook.Payload
                        .Replace(BeatPulseUIKeys.LIVENESS_BOOKMARK, livenessName)
                        .Replace(BeatPulseUIKeys.FAILURE_BOOKMARK, content);

                    var payload = new StringContent(targetWebHook.Payload, Encoding.UTF8, BeatPulseUIKeys.DEFAULT_RESPONSE_CONTENT_TYPE);

                    try
                    {
                        var response = await httpClient.PostAsync(webHookUri, payload);

                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError($"The failure notification is not executed successfully for {webHook.Name} webhook. The error code is {response.StatusCode}.");
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError($"The failure notification for {webHook.Name} is not executed successfully.", exception);
                    }
                }
            }
        }
    }
}
