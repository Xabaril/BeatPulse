using System;

namespace BeatPulse.Core
{
    public class LivenessResult
    {
        public string Name { get; private set; }

        public string Message { get; private set; }

        public string Exception { get; private set; }

        public TimeSpan Elapsed { get; private set; }

        public bool Run { get; private set; }

        public string Path { get; private set; }

        public bool IsHealthy { get; private set; }

        private LivenessResult() { }

        public LivenessResult SetEnforced(string name, string path, TimeSpan duration, bool detailedErrors = false)
        {
            Name = name;
            Path = path;
            Run = true;
            Elapsed = duration;

            if (Exception != null)
            {
                Message = BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE;

                if (!detailedErrors)
                {
                    Exception = null;
                }
            }

            return this;
        }

        public static LivenessResult Healthy(string message = BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE)
        {
            return new LivenessResult()
            {
                IsHealthy = true,
                Message = message
            };
        }

        public static LivenessResult UnHealthy(string message)
        {
            return new LivenessResult()
            {
                IsHealthy = false,
                Message = message
            };
        }

        public static LivenessResult UnHealthy(Exception exception)
        {
            return new LivenessResult()
            {
                IsHealthy = false,
                Exception = exception.Message
            };
        }

        public static LivenessResult TimedOut()
        {
            return new LivenessResult()
            {
                IsHealthy = false,
                Message = BeatPulseKeys.BEATPULSE_TIMEOUT_MESSAGE
            };
        }
    }
}
