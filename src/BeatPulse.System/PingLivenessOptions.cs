using System.Collections.Generic;

namespace BeatPulse.System
{
    public class PingLivenessOptions
    {
        internal Dictionary<string, (string Host, int TimeOut)> ConfiguredHosts { get; } = new Dictionary<string, (string, int)>();

        public PingLivenessOptions AddHost(string host, int timeout)
        {
            ConfiguredHosts.Add(host, (host, timeout));
            return this;
        }
    }
}
