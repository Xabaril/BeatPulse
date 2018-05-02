namespace BeatPulse
{
    public class BeatPulseKeys
    {
        public const string BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE = "The beat pulse check {0} is not working properly";
        public const string BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE = "OK";
        public const string BEATPULSE_DEFAULT_PATH = "hc";

        internal const string BEATPULSE_SELF_SEGMENT = "_self";
        internal const string BEATPULSE_SELF_NAME = "self";
        internal const string BEATPULSE_PATH_SEGMENT_NAME = "segment";
        internal const string BEATPULSE_INVALIDPATH_REASON = "Invalid BeatPulse path";
        internal const string BEATPULSE_TIMEOUT_MESSAGE = "Timeout";
        internal const string BEATPULSE_SERVICEUNAVAILABLE_REASON = "One or more liveness return a service unavailable";
    }
}
