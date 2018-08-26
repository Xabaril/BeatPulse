using System.Collections.Generic;

namespace BeatPulse.Network
{
    public class TcpLivenessOptions
    {
        internal List<(string host, int port)> ConfiguredHosts = new List<(string host, int port)>();

        public TcpLivenessOptions AddHost(string host, int port)
        {
            ConfiguredHosts.Add((host, port));
            return this;
        }
    }
}
