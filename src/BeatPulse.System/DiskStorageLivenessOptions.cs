using System.Collections.Concurrent;

namespace BeatPulse.System
{
    public class DiskStorageLivenessOptions
    {
        internal ConcurrentDictionary<string, (string DriveName, long MinimumFreeMegabytes)> ConfiguredDrives { get; } = new ConcurrentDictionary<string, (string, long)>();
        public bool AddDrive(string driveName, long minimumFreeMegabytes) => ConfiguredDrives.TryAdd(driveName, (driveName, minimumFreeMegabytes));        
    }
}
