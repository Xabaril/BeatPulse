using BeatPulse.Core;
using BeatPulse.System.Extensions;
using Microsoft.AspNetCore.Http;
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
        private DriveInfo[] _drives;

        public DiskStorageLiveness(DiskStorageLivenessOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
             _drives = DriveInfo.GetDrives();
            
            foreach(var configuredDrive in _options.ConfiguredDrives)
            {
                var (driveName, minDriveMegabytes) = configuredDrive.Value;
                var driveInfo = GetDriveInfo(driveName);

                if (!driveInfo.Exists)
                {
                    return Task.FromResult(livenessContext.CreateErrorResponse($"Configured drive {driveName} is not present on system"));
                }

                var successfulCheck = driveInfo.ActualFreeMegabytes >= minDriveMegabytes;
                
                if(!successfulCheck)
                {
                    return Task.FromResult(livenessContext.CreateErrorResponse($"Minimum configured megabytes for disk {driveName} is {minDriveMegabytes} but actual free space are {driveInfo.ActualFreeMegabytes} megabytes"));
                }
            }

            return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
        }

        private (bool Exists, long ActualFreeMegabytes) GetDriveInfo(string driveName)
        {
            var drive = _drives.FirstOrDefault(d => String.Equals(d.Name, driveName, StringComparison.InvariantCultureIgnoreCase));

            if (drive == null)
            {
                return (false, 0);
            }

            return (true, drive.AvailableFreeSpace / 1024 / 1024);
        }        
    }   
}
