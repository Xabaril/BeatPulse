using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Network.Core
{
    public class ImapConnectionOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public bool AllowInvalidRemoteCertificates { get; set; }
    }
}
