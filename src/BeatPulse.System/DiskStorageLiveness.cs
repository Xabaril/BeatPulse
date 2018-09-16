using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.System
{
    public class DiskStorageLiveness : IHealthCheck
    {
        private readonly DiskStorageLivenessOptions _options;

        public DiskStorageLiveness(DiskStorageLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var configuredDrives = _options.ConfiguredDrives.Values;

            foreach (var item in configuredDrives)
            {
                var systemDriveInfo = GetSystemDriveInfo(item.DriveName);

                if (systemDriveInfo.Exists)
                {
                    if (systemDriveInfo.ActualFreeMegabytes < item.MinimumFreeMegabytes)
                    {
                        return Task.FromResult(HealthCheckResult.Failed($"Minimum configured megabytes for disk {item.DriveName} is {item.MinimumFreeMegabytes} but actual free space are {systemDriveInfo.ActualFreeMegabytes} megabytes"));
                    }
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Failed($"Configured drive {item.DriveName} is not present on system"));
                }
            }

            return Task.FromResult(HealthCheckResult.Passed());
        }
        private (bool Exists, long ActualFreeMegabytes) GetSystemDriveInfo(string driveName)
        {
            var driveInfo = DriveInfo.GetDrives()
                .FirstOrDefault(drive => String.Equals(drive.Name, driveName, StringComparison.InvariantCultureIgnoreCase));

            if (driveInfo != null)
            {
                return (true, driveInfo.AvailableFreeSpace / 1024 / 1024);
            }

            return (false, 0);
        }
    }
}
