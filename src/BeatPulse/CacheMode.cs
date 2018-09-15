namespace BeatPulse
{
    /// <summary>
    /// The BeatPulse cache mode.
    /// </summary>
    public enum CacheMode
        :short
    {
        /// <summary>
        /// Only use http cache headers.
        /// </summary>
        Header,
        /// <summary>
        /// Use internal server memory to store BeatPulse health check results.
        /// </summary>
        ServerMemory,
        /// <summary>
        /// Use http cache headers and internal server memory to store Beatpulse health check results.
        /// </summary>
        HeaderAndServerMemory
    }

    /// <summary>
    /// <see cref="CacheMode"/> extension methods for BeatPulse.
    /// </summary>
    static class CacheModeExtensions
    {
        /// <summary>
        /// Check if cache mode is configured as Header.
        /// </summary>
        /// <param name="mode"><see cref="CacheMode"/></param>
        /// <returns>True if cache mode is configured as Header.</returns>
        public static bool UseHeader(this CacheMode mode) => mode == CacheMode.Header || mode == CacheMode.HeaderAndServerMemory;

        /// <summary>
        /// Check if cache mode is configured as Server memory.
        /// </summary>
        /// <param name="mode"><see cref="CacheMode"/></param>
        /// <returns>True if cache mode is configured as Server memory.</returns>
        public static bool UseServerMemory(this CacheMode mode) => mode == CacheMode.ServerMemory || mode == CacheMode.HeaderAndServerMemory;
    }
}
