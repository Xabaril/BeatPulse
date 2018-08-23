using System.Collections.Generic;

namespace BeatPulse.Network
{
    public class SftpLivenessOptions
    {
        internal Dictionary<string, SftpConfiguration> ConfiguredHosts { get; } = new Dictionary<string, SftpConfiguration>();

        public SftpLivenessOptions AddHost(SftpConfiguration sftpConfiguration)
        {
            ConfiguredHosts.Add(sftpConfiguration.Host, sftpConfiguration);
            return this;
        }
    }
}
