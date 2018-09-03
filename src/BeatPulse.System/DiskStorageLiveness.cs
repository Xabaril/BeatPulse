using BeatPulse.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.System
{
    public class DiskStorageLiveness : IBeatPulseLiveness
    {
        private readonly DiskStorageLivenessOptions _options;

        public DiskStorageLiveness(DiskStorageLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        { 
            try
            {
                var configuredDrives = _options.ConfiguredDrives.Values;

                foreach (var item in configuredDrives)
                {
                    var systemDriveInfo = GetSystemDriveInfo(item.DriveName);

                    if (systemDriveInfo.Exists)
                    {
                        if (systemDriveInfo.ActualFreeMegabytes < item.MinimumFreeMegabytes)
                        {
                            return Task.FromResult(($"Minimum configured megabytes for disk {item.DriveName} is {item.MinimumFreeMegabytes} but actual free space are {systemDriveInfo.ActualFreeMegabytes} megabytes", false));
                        }
                    }
                    else
                    {
                        return Task.FromResult(($"Configured drive {item.DriveName} is not present on system",false));
                    }
                }

                return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
            }
            catch (Exception ex)
            {
                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                       : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return Task.FromResult((message, false));
            }
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
