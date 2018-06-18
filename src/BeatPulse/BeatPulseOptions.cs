using System;

namespace BeatPulse
{
    public interface IBeatPulseOptions
    {
        IBeatPulseOptions ConfigureOutputCache(int seconds, CacheMode mode = CacheMode.Header);
        IBeatPulseOptions ConfigureDetailedOutput(bool detailedOutput = true);
        IBeatPulseOptions ConfigurePath(string path);
        IBeatPulseOptions ConfigureTimeout(int milliseconds);
    }

    public class BeatPulseOptions : IBeatPulseOptions
    {
        public bool DetailedOutput { get; private set; }
        public string BeatPulsePath { get; private set; }
        public int Timeout { get; private set; }
        public int CacheDuration { get; private set; }
        public bool CacheOutput { get; private set; }
        public CacheMode CacheMode { get; private set; }

        public BeatPulseOptions()
        {
            DetailedOutput = false;
            BeatPulsePath = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
            Timeout = -1; // wait infinitely
            CacheOutput = false;
            CacheDuration = 0;
            CacheMode = CacheMode.Header;
        }

        public IBeatPulseOptions ConfigureOutputCache(int seconds, CacheMode mode = CacheMode.Header)
        {
            CacheDuration = seconds;
            CacheOutput = true;
            CacheMode = mode;

            return this;
        }

        public IBeatPulseOptions ConfigureDetailedOutput(bool detailedOutput = true)
        {
            DetailedOutput = detailedOutput;

            return this;
        }

        public IBeatPulseOptions ConfigurePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            BeatPulsePath = path;

            return this;
        }

        public IBeatPulseOptions ConfigureTimeout(int milliseconds)
        {
            Timeout = milliseconds;

            return this;
        }
    }
}