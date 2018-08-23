using BeatPulse.Core;
using BeatPulse.System;
using System;

namespace BeatPulse
{
    public static class ServiceCollectionExtensions
    {
        public static BeatPulseContext AddDiskStorageLiveness(this BeatPulseContext context, Action<DiskStorageLivenessOptions> options, string name = nameof(DiskStorageLiveness), string defaultPath = "diskstorage")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);

                var diskStorageLivenesOptions = new DiskStorageLivenessOptions();
                options?.Invoke(diskStorageLivenesOptions);

                setup.UseLiveness(new DiskStorageLiveness(diskStorageLivenesOptions));
            });
        }
    }
}
