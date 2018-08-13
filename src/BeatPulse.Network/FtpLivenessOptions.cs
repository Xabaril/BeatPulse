using System.Collections.Generic;
using System.Net;

namespace BeatPulse.Network
{
    public class FtpLivenessOptions
    {
        internal Dictionary<string, (string host, NetworkCredential credentials)> ConfiguredHosts { get; } = new Dictionary<string, (string, NetworkCredential)>();

        public FtpLivenessOptions AddHost(string host, NetworkCredential credentials = null)
        {
            ConfiguredHosts.Add(host, (host, credentials));
            return this;
        }
    }
}
