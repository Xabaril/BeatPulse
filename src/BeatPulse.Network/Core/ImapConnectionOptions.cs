using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Network.Core
{
    public class ImapConnectionOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public ImapConnectionType ConnectionType { get; set; } = ImapConnectionType.AUTO;
        public bool AllowInvalidRemoteCertificates { get; set; }
    }
}
