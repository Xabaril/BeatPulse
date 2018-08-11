using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.System
{
    public class PingLivenessOptions
    {
        internal ConcurrentDictionary<string, (string Host, int TimeOut)> RegisteredHosts { get; } = new ConcurrentDictionary<string, (string, int)>();
        public bool AddHost(string host, int timeout) => RegisteredHosts.TryAdd(host, (host, timeout));        
    }
}
