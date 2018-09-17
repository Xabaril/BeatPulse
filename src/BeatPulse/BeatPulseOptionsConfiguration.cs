
namespace BeatPulse
{
    public class BeatPulseOptionsConfiguration: BeatPulseOptions
    {
        public new bool DetailedOutput
        {
            get => base.DetailedOutput;
            set => base.DetailedOutput = value;
        }

        public new string Path
        {
            get => base.Path;
            set => base.Path = value;
        }

        public new int? Port
        {
            get => base.Port;
            set => base.Port = value;
        }

        public new int Timeout
        {
            get => base.Timeout;
            set => base.Timeout = value;
        }

        public new int CacheDuration
        {
            get => base.CacheDuration;
            set => base.Timeout = value;
        }

        public new bool CacheOutput
        {
            get => base.CacheOutput;
            set => base.CacheOutput = value;
        }

        public new CacheMode CacheMode
        {
            get => base.CacheMode;
            set => base.CacheMode = value;
        }

        public new bool DetailedErrors
        {
            get => base.DetailedErrors;
            set => base.DetailedErrors = value;
        }
    }
}
