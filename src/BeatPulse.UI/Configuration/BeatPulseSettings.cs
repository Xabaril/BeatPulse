using System.Collections.Generic;

namespace BeatPulse.UI.Configuration
{
    class BeatPulseSettings
    {
        public List<LivenessConfigurationSetting> Liveness { get; set; }

        public List<WebHookNotification> Webhooks { get; set; } = new List<WebHookNotification>();

        public int EvaluationTimeOnSeconds { get; set; } = 10;

        public int MinimumSecondsBetweenFailureNotifications { get; set; } = 60 * 10;
    }

    class LivenessConfigurationSetting
    {
        public string Name { get; set; }

        public string Uri { get; set; }
    }

    class WebHookNotification
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Payload { get; set; }
    }
}
