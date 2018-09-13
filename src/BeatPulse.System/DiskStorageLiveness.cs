using BeatPulse.Core;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DiskStorageLiveness> _logger;

        public DiskStorageLiveness(DiskStorageLivenessOptions options, ILogger<DiskStorageLiveness> logger = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(DiskStorageLiveness)} is checking configured drives.");

                var configuredDrives = _options.ConfiguredDrives.Values;

                foreach (var item in configuredDrives)
                {
                    var systemDriveInfo = GetSystemDriveInfo(item.DriveName);

                    if (systemDriveInfo.Exists)
                    {
                        if (systemDriveInfo.ActualFreeMegabytes < item.MinimumFreeMegabytes)
                        {
                            _logger?.LogWarning($"The {nameof(DiskStorageLiveness)} check fail for drive {item.DriveName}.");

                            return Task.FromResult(
                                LivenessResult.UnHealthy($"Minimum configured megabytes for disk {item.DriveName} is {item.MinimumFreeMegabytes} but actual free space are {systemDriveInfo.ActualFreeMegabytes} megabytes"));
                        }
                    }
                    else
                    {
                        _logger?.LogWarning($"{nameof(DiskStorageLiveness)} is checking a not present disk {item.DriveName} on system.");

                        return Task.FromResult(
                            LivenessResult.UnHealthy($"Configured drive {item.DriveName} is not present on system"));
                    }
                }

                _logger?.LogDebug($"The {nameof(DiskStorageLiveness)} check success.");

                return Task.FromResult(
                    LivenessResult.Healthy());
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(DiskStorageLiveness)} check fail with the exception {ex.ToString()}.");

                return Task.FromResult(
                    LivenessResult.UnHealthy(ex, showDetailedErrors: context.ShowDetailedErrors));
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
