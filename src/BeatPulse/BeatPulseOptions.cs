using System;

namespace BeatPulse
{
    /// <summary>
    /// BeatPulse options 
    /// </summary>
    public class BeatPulseOptions
    {
        protected internal bool DetailedOutput { get; protected set; }

        protected internal string Path { get; protected set; }

        protected internal int? Port { get; protected set; }

        protected internal int Timeout { get; protected set; }

        protected internal int CacheDuration { get; protected set; }

        protected internal bool CacheOutput { get; protected set; }

        protected internal CacheMode CacheMode { get; protected set; }

        protected internal bool DetailedErrors { get; protected set; }

        /// <summary>
        /// Create a instance of <see cref="BeatPulseOptions"/>
        /// </summary>
        public BeatPulseOptions()
        {
            DetailedOutput = false;
            Path = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
            Port = null;
            Timeout = -1; // wait infinitely
            CacheOutput = false;
            CacheDuration = 0;
            CacheMode = CacheMode.Header;
            DetailedErrors = false;
        }

        private BeatPulseOptions(bool detailedOutput, string path, int? port, int timeout, bool cacheoutput, int cacheDuration, bool detailedErrors, CacheMode cacheMode)
        {
            DetailedOutput = detailedOutput;
            Path = path;
            Port = port;
            Timeout = timeout;
            CacheOutput = cacheoutput;
            CacheDuration = cacheDuration;
            CacheMode = cacheMode;
            DetailedErrors = detailedErrors;
        }

        public BeatPulseOptions ConfigureOutputCache(int seconds, CacheMode mode = CacheMode.Header)
        {
            CacheDuration = seconds;
            CacheOutput = true;
            CacheMode = mode;

            return this;
        }

        /// <summary>
        /// Configured if check results include detailed information and exception messages.
        /// </summary>
        /// <param name="detailedOutput">If true check results include detailed information.</param>
        /// <param name="includeExceptionMessages">If true exception messages are included.</param>
        /// <returns><see cref="BeatPulseOptions"/></returns>
        public BeatPulseOptions ConfigureDetailedOutput(bool detailedOutput = true, bool includeExceptionMessages = false)
        {
            DetailedOutput = detailedOutput;
            DetailedErrors = detailedOutput ? includeExceptionMessages : false;

            return this;
        }

        /// <summary>
        /// Configure the BeatPulse path on wich provide BeatPulse health check results.
        /// </summary>
        /// <param name="path">The path to be configured.</param>
        /// <returns><see cref="BeatPulseOptions"/></returns>
        public BeatPulseOptions ConfigurePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;

            return this;
        }

        /// <summary>
        /// Configure a management port for BeatPulse.
        /// </summary>
        /// <param name="port">The port on wich BeatPulse provide health check results</param>
        /// <returns><see cref="BeatPulseOptions"/></returns>
        public BeatPulseOptions ConfigurePort(int port)
        {
            Port = port;

            return this;
        }

        /// <summary>
        /// Configure the maximun duration on milliseconds of health check request. 
        /// </summary>
        /// <param name="milliseconds">Maximun duration on milliseconds.</param>
        /// <returns><see cref="BeatPulseOptions"/></returns>
        public BeatPulseOptions ConfigureTimeout(int milliseconds)
        {
            Timeout = milliseconds;

            return this;
        }

        internal BeatPulseOptions DeepClone()
        {
            return new BeatPulseOptions(
               DetailedOutput,
               Path,
               Port,
               Timeout,
               CacheOutput,
               CacheDuration,
               DetailedErrors,
               CacheMode);
        }
    }
}