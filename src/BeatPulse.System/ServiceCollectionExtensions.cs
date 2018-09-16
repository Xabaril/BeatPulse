using System;
using System.Diagnostics;
using BeatPulse.System;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class ServiceCollectionExtensions
    {
        public static IHealthChecksBuilder AddDiskStorageLiveness(this IHealthChecksBuilder builder, Action<DiskStorageLivenessOptions> options, string name = nameof(DiskStorageLiveness), string defaultPath = "diskstorage")
        {
            var diskStorageLivenesOptions = new DiskStorageLivenessOptions();
            options?.Invoke(diskStorageLivenesOptions);
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new DiskStorageLiveness(diskStorageLivenesOptions));
        }

        public static IHealthChecksBuilder AddPrivateMemoryLiveness(this IHealthChecksBuilder builder, long maximumMemoryBytes, string name = Constants.PrivateMemoryLiveness, string defaultPath = "privatememory")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new MaximumValueLiveness<long>(maximumMemoryBytes, () => Process.GetCurrentProcess().PrivateMemorySize64));
        }

        public static IHealthChecksBuilder AddWorkingSetLiveness(this IHealthChecksBuilder builder, long maximumMemoryBytes, string name = Constants.WorkingSetLiveness, string defaultPath = "workingset")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new MaximumValueLiveness<long>(maximumMemoryBytes, () => Process.GetCurrentProcess().WorkingSet64));
        }

        public static IHealthChecksBuilder AddVirtualMemorySizeLiveness(this IHealthChecksBuilder builder, long maximumMemoryBytes, string name = Constants.VirtualMemorySizeLiveness, string defaultPath = "virtualmemorysize")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new MaximumValueLiveness<long>(maximumMemoryBytes, () => Process.GetCurrentProcess().VirtualMemorySize64));
        }
    }
}
