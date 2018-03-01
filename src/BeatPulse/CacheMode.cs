using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse
{
    public enum CacheMode
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
