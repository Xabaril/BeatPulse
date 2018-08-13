using BeatPulse.Core;
using System;

namespace BeatPulse.Network
{
    public static class ServiceCollectionExtensions
    {
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
                options?.Invoke(ftpLivenessOptions);;

                setup.UseLiveness(new FtpLiveness(ftpLivenessOptions));
            });
        }
    }
}
