using System.Collections.Generic;

namespace BeatPulse.Network
{
    public class DnsResolveOptions
    {
        private readonly string _dnsHost;
        internal Dictionary<string, DnsRegistration> ConfigureHosts = new Dictionary<string, DnsRegistration>();
        
        public void AddHost(string host, DnsRegistration registration)
        {
            ConfigureHosts.Add(host, registration);
        }
    }    
}
