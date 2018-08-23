using System.Collections.Generic;

namespace BeatPulse.Network
{
    public class SftpLivenessOptions
    {
        internal Dictionary<string, SftpHostConfiguration> ConfiguredHosts { get; } = new Dictionary<string, SftpHostConfiguration>();

        public SftpLivenessOptions AddHost(SftpHostConfiguration sftpHostConfiguration)
        {
            ConfiguredHosts.Add(sftpHostConfiguration.Host, sftpHostConfiguration);
            return this;
        }
    }
}
