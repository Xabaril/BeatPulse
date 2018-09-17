using System;

namespace BeatPulse.Core
{
    /// <summary>
    /// The liveness execution result.
    /// </summary>
    public class LivenessResult
    {
        /// <summary>
        /// The name of liveness that produce this result.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The liveness execution result message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The liveness execution exception message.
        /// </summary>
        public string Exception { get; private set; }

        /// <summary>
        /// The execution duration.
        /// </summary>
        public TimeSpan Elapsed { get; private set; }

        /// <summary>
        /// True if liveness is executed, else false.
        /// </summary>
        public bool Run { get; private set; }

        /// <summary>
        /// The configured path for the liveness.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The liveness status result.
        /// </summary>
        public bool IsHealthy { get; private set; }

        private LivenessResult() { }

        /// <summary>
        /// Finish liveness result with specified information.
        /// </summary>
        /// <param name="name">The configured liveness name.</param>
        /// <param name="path">The configured liveness path.</param>
        /// <param name="duration">The liveness execution duration.</param>
        /// <param name="detailedErrors">True if detailed errors configuration value.</param>
        /// <returns><see cref="LivenessResult"/>.</returns>
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

        /// <summary>
        /// Set Livenes execution as Healthy.
        /// </summary>
        /// <param name="message">The liveness execution message.</param>
        /// <returns><see cref="LivenessResult"/>.</returns>
        public static LivenessResult Healthy(string message = BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE)
        {
            return new LivenessResult()
            {
                IsHealthy = true,
                Message = message
            };
        }

        /// <summary>
        /// Set Livenes execution as unhealty.
        /// </summary>
        /// <param name="message">The unhealthy message.</param>
        /// <returns><see cref="LivenessResult"/>.</returns>
        public static LivenessResult UnHealthy(string message)
        {
            return new LivenessResult()
            {
                IsHealthy = false,
                Message = message
            };
        }

        /// <summary>
        /// Set Livenes execution as unhealty.
        /// </summary>
        /// <param name="exception">The exception thrown by liveness.</param>
        /// <returns><see cref="LivenessResult"/>.</returns>
        public static LivenessResult UnHealthy(Exception exception)
        {
            return new LivenessResult()
            {
                IsHealthy = false,
                Exception = exception.Message
            };
        }

        /// <summary>
        /// Set Livenes execution as Timeout.
        /// </summary>
        /// <returns><see cref="LivenessResult"/>.</returns>
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
