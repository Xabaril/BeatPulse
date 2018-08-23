using System.Collections.Generic;
using System.Net;

namespace BeatPulse.Network
{
    public class FtpLivenessOptions
    {
        internal Dictionary<string, (string host, bool createFile, NetworkCredential credentials)> Hosts { get; } = new Dictionary<string, (string, bool, NetworkCredential)>();

        public FtpLivenessOptions AddHost(string host, bool createFile = false, NetworkCredential credentials = null)
        {
            Hosts.Add(host, (host, createFile, credentials));

            return this;
        }
    }
}