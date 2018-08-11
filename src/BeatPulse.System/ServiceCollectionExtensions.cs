using BeatPulse.Core;
using BeatPulse.System;
using System;


namespace BeatPulse
{
    public static class ServiceCollectionExtensions
    {
        public static BeatPulseContext AddPingLiveness(this BeatPulseContext context, Action<PingLivenessOptions> options, string name = nameof(PingLiveness), string defaultPath = "ping") {

            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var pingLivenessOptions = new PingLivenessOptions();
                options(pingLivenessOptions);

                setup.UseLiveness(new PingLiveness(pingLivenessOptions));
            });
        }

        public static BeatPulseContext AddDiskStorageLiveness(this BeatPulseContext context, Action<DiskStorageLivenessOptions> options, string name = nameof(DiskStorageLiveness), string defaultPath = "diskstorage")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var diskStorageLivenesOptions = new DiskStorageLivenessOptions();
                options(diskStorageLivenesOptions);

                setup.UseLiveness(new DiskStorageLiveness(diskStorageLivenesOptions));
            });
        }
    }
}
