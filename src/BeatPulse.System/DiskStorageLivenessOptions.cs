using System.Collections.Generic;

namespace BeatPulse.System
{
    public class DiskStorageLivenessOptions
    {
        internal Dictionary<string, (string DriveName, long MinimumFreeMegabytes)> ConfiguredDrives { get; } = new Dictionary<string, (string DriveName, long MinimumFreeMegabytes)>();

        public DiskStorageLivenessOptions AddDrive(string driveName, long minimumFreeMegabytes = 1)
        { 
            ConfiguredDrives.Add(driveName, (driveName, minimumFreeMegabytes));
            return this;
        }
    }
}
