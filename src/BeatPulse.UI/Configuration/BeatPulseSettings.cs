using System.Collections.Generic;

namespace BeatPulse.UI.Configuration
{
    class BeatPulseSettings
    {
        public List<LivenessConfigurationSetting> Liveness { get; set; }

        public string WebHookNotificationUri { get; set; }

        public int EvaluationTimeOnSeconds { get; set; } = 10;

        public int MinimumSecondsBetweenFailureNotifications { get; set; } = 60 * 10;
    }

    class LivenessConfigurationSetting
    {
        public string Name { get; set; }

        public string Uri { get; set; }
    }
}
