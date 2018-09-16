using System;
using BeatPulse.Network;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class ServiceCollectionExtensions
    {
        public static IHealthChecksBuilder AddPingLiveness(this IHealthChecksBuilder builder, Action<PingLivenessOptions> options, string name = nameof(PingLiveness), string defaultPath = "ping")
        {
            var pingLivenessOptions = new PingLivenessOptions();
            options?.Invoke(pingLivenessOptions);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new PingLiveness(pingLivenessOptions));
        }
        public static IHealthChecksBuilder AddSftpLiveness(this IHealthChecksBuilder builder, Action<SftpLivenessOptions> options,
            string name = nameof(SftpLiveness), string defaultPath = "sftp")
        {
            var sftpLivenessOptions = new SftpLivenessOptions();
            options?.Invoke(sftpLivenessOptions);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new SftpLiveness(sftpLivenessOptions));
        }

        public static IHealthChecksBuilder AddFtpLiveness(this IHealthChecksBuilder builder, Action<FtpLivenessOptions> options,
            string name = nameof(FtpLiveness), string defaultPath = "ftp")
        {
            var ftpLivenessOptions = new FtpLivenessOptions();
            options?.Invoke(ftpLivenessOptions);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new FtpLiveness(ftpLivenessOptions));
        }

        public static IHealthChecksBuilder AddDnsResolveLiveness(this IHealthChecksBuilder builder, Action<DnsResolveOptions> options,
             string name = nameof(DnsResolveOptions), string defaultPath = "dns")
        {
            var dnsResolveOptions = new DnsResolveOptions();
            options?.Invoke(dnsResolveOptions);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new DnsResolveLiveness(dnsResolveOptions));
        }

        public static IHealthChecksBuilder AddImapLiveness(this IHealthChecksBuilder builder, Action<ImapLivenessOptions> options,
          string name = nameof(ImapLiveness), string defaultPath = "imap")
        {
            var imapOptions = new ImapLivenessOptions();
            options?.Invoke(imapOptions);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new ImapLiveness(imapOptions));
        }

        public static IHealthChecksBuilder AddSmtpLiveness(this IHealthChecksBuilder builder, Action<SmtpLivenessOptions> options,
         string name = nameof(SmtpLiveness), string defaultPath = "smtp")
        {
            var smtpOptions = new SmtpLivenessOptions();
            options?.Invoke(smtpOptions);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new SmtpLiveness(smtpOptions));
        }
    }
}
