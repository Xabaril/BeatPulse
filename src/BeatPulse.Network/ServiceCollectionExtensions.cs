using BeatPulse.Core;
using BeatPulse.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

                setup.UseFactory(sp => new PingLiveness(pingLivenessOptions, sp.GetService<ILogger<PingLiveness>>()));
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

                setup.UseFactory(sp => new SftpLiveness(sftpLivenessOptions, sp.GetService<ILogger<SftpLiveness>>()));
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

                setup.UseFactory(sp => new FtpLiveness(ftpLivenessOptions, sp.GetService<ILogger<FtpLiveness>>()));
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

                setup.UseFactory(sp => new DnsResolveLiveness(dnsResolveOptions, sp.GetService<ILogger<DnsResolveLiveness>>()));
            });
        }

        public static BeatPulseContext AddImapLiveness(this BeatPulseContext context, Action<ImapLivenessOptions> options,
          string name = nameof(ImapLiveness), string defaultPath = "imap")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var imapOptions = new ImapLivenessOptions();
                options?.Invoke(imapOptions);

                setup.UseFactory(sp => new ImapLiveness(imapOptions, sp.GetService<ILogger<ImapLiveness>>()));
            });
        }

        public static BeatPulseContext AddSmtpLiveness(this BeatPulseContext context, Action<SmtpLivenessOptions> options,
         string name = nameof(SmtpLiveness), string defaultPath = "smtp")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var smtpOptions = new SmtpLivenessOptions();
                options?.Invoke(smtpOptions);

                setup.UseFactory(sp=>new SmtpLiveness(smtpOptions, sp.GetService<ILogger<SmtpLiveness>>()));
            });
        }
    }
}
