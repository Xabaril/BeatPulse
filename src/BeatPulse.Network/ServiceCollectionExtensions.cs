using BeatPulse.Core;
using BeatPulse.Network;
using System;

namespace BeatPulse
{
    public static class ServiceCollectionExtensions
    {
        public static BeatPulseContext AddPingLiveness(this BeatPulseContext context, Action<PingLivenessOptions> options, string name = nameof(PingLiveness), string defaultPath = "ping")
        {

            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var pingLivenessOptions = new PingLivenessOptions();
                options?.Invoke(pingLivenessOptions);

                setup.UseLiveness(new PingLiveness(pingLivenessOptions));
            });
        }
        public static BeatPulseContext AddSftpLiveness(this BeatPulseContext context, Action<SftpLivenessOptions> options,
            string name = nameof(SftpLiveness), string defaultPath = "sftp")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var sftpLivenessOptions = new SftpLivenessOptions();
                options?.Invoke(sftpLivenessOptions);

                setup.UseLiveness(new SftpLiveness(sftpLivenessOptions));
            });
        }

        public static BeatPulseContext AddFtpLiveness(this BeatPulseContext context, Action<FtpLivenessOptions> options,
            string name = nameof(FtpLiveness), string defaultPath = "ftp")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var ftpLivenessOptions = new FtpLivenessOptions();
                options?.Invoke(ftpLivenessOptions);

                setup.UseLiveness(new FtpLiveness(ftpLivenessOptions));
            });
        }

        public static BeatPulseContext AddDnsResolveLiveness(this BeatPulseContext context, Action<DnsResolveOptions> options,
             string name = nameof(DnsResolveOptions), string defaultPath = "dns")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var dnsResolveOptions = new DnsResolveOptions();
                options?.Invoke(dnsResolveOptions);

                setup.UseLiveness(new DnsResolveLiveness(dnsResolveOptions));
            });
        }
    }
}
