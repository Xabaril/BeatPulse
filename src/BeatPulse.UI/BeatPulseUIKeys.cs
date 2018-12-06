namespace BeatPulse.UI
{
    class BeatPulseUIKeys
    {
        internal const string BEATPULSE_DEFAULT_PATH = "hc";
        internal const string BEATPULSE_DEFAULT_DISCOVERY_LABEL = "beatpulse";
        internal const string BEATPULSEUI_SECTION_SETTING_KEY = "BeatPulse-UI";
        internal const string BEATPULSEUI_KUBERNETES_DISCOVERY_SETTING_KEY = "BeatPulse-UI:KubernetesDiscoveryService";
        internal const string BEATPULSEUI_MAIN_UI_RESOURCE = "index.html";
        internal const string BEATPULSEUI_MAIN_UI_API_TARGET = "#apiPath#";
        internal const string BEATPULSEUI_WEBHOOKS_API_TARGET = "#webhookPath#";
        internal const string BEATPULSEUI_MAIN_UI_RESOURCE_TARGET = "#uiResourcePath#";
        internal const string DEFAULT_RESPONSE_CONTENT_TYPE = "application/json";
        internal const string LIVENESS_BOOKMARK = "[[LIVENESS]]";
        internal const string FAILURE_BOOKMARK = "[[FAILURE]]";
    }
}
