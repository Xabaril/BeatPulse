using System;

namespace BeatPulse
{
    public class BeatPulseOptions
    {
        internal bool DetailedOutput { get; private set; }

        internal string BeatPulsePath { get; private set; }

        internal int Timeout { get; private set; }

        internal int CacheDuration { get; private set; }

        internal bool CacheOutput { get; private set; }

        public BeatPulseOptions()
        {
            DetailedOutput = false;
            BeatPulsePath = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
            Timeout = -1; // wait infinitely
            CacheOutput = false;
            CacheDuration = 0;
        }

        public BeatPulseOptions EnableOutputCache(int seconds)
        {
            CacheDuration = seconds;
            CacheOutput = true;

            return this;
        }

        public BeatPulseOptions EnableDetailedOutput()
        {
            DetailedOutput = true;

            return this;
        }

        public BeatPulseOptions SetAlternatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            BeatPulsePath = path;

            return this;
        }

        public BeatPulseOptions SetTimeout(int milliseconds)
        {
            Timeout = milliseconds;

            return this;
        }
    }
}
