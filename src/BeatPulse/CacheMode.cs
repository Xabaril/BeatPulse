namespace BeatPulse
{
    public enum CacheMode
        :short
    {
        Header,
        ServerMemory,
        HeaderAndServerMemory
    }

    static class CacheModeExtensions
    {
        public static bool UseHeader(this CacheMode mode) => mode == CacheMode.Header || mode == CacheMode.HeaderAndServerMemory;

        public static bool UseServerMemory(this CacheMode mode) => mode == CacheMode.ServerMemory || mode == CacheMode.HeaderAndServerMemory;
    }
}
