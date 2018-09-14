using System;

namespace BeatPulse
{
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

        public BeatPulseOptions ConfigureDetailedOutput(bool detailedOutput = true, bool includeExceptionMessages = false)
        {
            DetailedOutput = detailedOutput;
            DetailedErrors = detailedOutput ? includeExceptionMessages : false;

            return this;
        }

        public BeatPulseOptions ConfigurePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;

            return this;
        }

        public BeatPulseOptions ConfigurePort(int port)
        {
            Port = port;

            return this;
        }

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