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
                var targetWebHook = new WebHookNotification()
                {
                    Name = webHook.Name,
                    Uri = webHook.Uri                    
                };

                targetWebHook.Payload = webHook.Payload
                    .Replace(BeatPulseUIKeys.LIVENESS_BOOKMARK, livenessName)
                    .Replace(BeatPulseUIKeys.FAILURE_BOOKMARK, content);

                await NotifyWebhookPayLoad(targetWebHook, targetWebHook.Payload);
            }
        }

        public async Task NotifyLivenessRestored(string livenessName, string context)
        {
            foreach (var webHook in _settings.Webhooks)
            {
                var targetWebHook = new WebHookNotification()
                {
                    Name = webHook.Name,
                    Uri = webHook.Uri                 
                };

                targetWebHook.RestoredPayload = webHook.RestoredPayload
                    .Replace(BeatPulseUIKeys.LIVENESS_BOOKMARK, livenessName);

                await NotifyWebhookPayLoad(targetWebHook, targetWebHook.RestoredPayload);
            }
        }

        private async Task NotifyWebhookPayLoad(WebHookNotification webHook, string payloadContent)
        {
            if (webHook.Uri == null || !Uri.TryCreate(webHook.Uri, UriKind.Absolute, out Uri webHookUri))
            {
                _logger.LogWarning($"The web hook notification uri is not stablished or is not an absolute Uri ({webHook.Name}). Set the webhook uri value on BeatPulse setttings.");

                return;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var payload = new StringContent(payloadContent, Encoding.UTF8, BeatPulseUIKeys.DEFAULT_RESPONSE_CONTENT_TYPE);
                    var response = await httpClient.PostAsync(webHookUri, payload);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError($"The webhook notification has not executed successfully for {webHook.Name} webhook. The error code is {response.StatusCode}.");
                    }
                }

            }
            catch (Exception exception)
            {
                _logger.LogError($"The failure notification for {webHook.Name} has not executed successfully.", exception);
            }

        }
    }
}
