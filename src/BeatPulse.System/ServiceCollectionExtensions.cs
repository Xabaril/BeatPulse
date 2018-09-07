using BeatPulse.Core;
using BeatPulse.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

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

                setup.UseFactory(sp => new DiskStorageLiveness(diskStorageLivenesOptions, sp.GetService<ILogger<DiskStorageLiveness>>()));
            });
        }

        public static BeatPulseContext AddPrivateMemoryLiveness(this BeatPulseContext context, long maximumMemoryBytes, string name = Constants.PrivateMemoryLiveness, string defaultPath = "privatememory")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new MaximumValueLiveness<long>(maximumMemoryBytes, () => Process.GetCurrentProcess().PrivateMemorySize64));
            });
        }

        public static BeatPulseContext AddWorkingSetLiveness(this BeatPulseContext context, long maximumMemoryBytes, string name = Constants.WorkingSetLiveness, string defaultPath = "workingset")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new MaximumValueLiveness<long>(maximumMemoryBytes, () => Process.GetCurrentProcess().WorkingSet64));
            });
        }

        public static BeatPulseContext AddVirtualMemorySizeLiveness(this BeatPulseContext context, long maximumMemoryBytes, string name = Constants.VirtualMemorySizeLiveness, string defaultPath = "virtualmemorysize")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new MaximumValueLiveness<long>(maximumMemoryBytes, () => Process.GetCurrentProcess().VirtualMemorySize64));
            });
        }
    }
}
