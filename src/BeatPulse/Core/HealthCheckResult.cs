using System;
using System.Diagnostics;

namespace BeatPulse.Core
{
    public class HealthCheckResult
    {
        private readonly Stopwatch _watcher = new Stopwatch();

        public string Name { get; private set; }

        public string Message { get; private set; }

        public long MilliSeconds {get; private set;}

        public bool Run { get; private set; }

        public bool IsHealthy { get; private set; }

        public HealthCheckResult(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Run = false;
            IsHealthy = false;
        }

        internal void StartCounter()
        {
            _watcher.Restart();
        }

        internal void StopCounter(string message, bool healthy)
        {
            _watcher.Stop();

            MilliSeconds = _watcher.ElapsedMilliseconds;
            Run = true;
            Message = message;
            IsHealthy = healthy;
        }
    }
}
