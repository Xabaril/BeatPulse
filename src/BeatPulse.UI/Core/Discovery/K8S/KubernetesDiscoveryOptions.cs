﻿namespace BeatPulse.UI.Core.Discovery.K8S
{
    public class KubernetesDiscoveryOptions
    {
        public bool Enabled { get; set; } = false;
        public string ClusterHost { get; set; }
        public string BeatpulsePath { get; set; } = BeatPulseUIKeys.BEATPULSE_DEFAULT_PATH;
        public string ServicesLabel { get; set; } = BeatPulseUIKeys.BEATPULSE_DEFAULT_DISCOVERY_LABEL;
        public string Token { get; set; }
        public int RefreshTimeOnSeconds { get; set; } = 300;
    }
}
