using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BeatPulse.Core
{
    public class HealthCheckMessage
    {
        private readonly Stopwatch _watcher;
        public string Name { get; }
        public string Message { get; private set; }

        public long MilliSeconds {get; private set;}

        public bool Run { get; private set; }
        public bool IsHealthy { get; private set; }

        public HealthCheckMessage(string name)
        {
            Name = name;
            Run = false;
            _watcher = new Stopwatch();
            IsHealthy = false;
        }

        internal void StartCounter()
        {
            _watcher.Restart();
        }

        internal void StopCounter(string msg, bool healthy)
        {
            _watcher.Stop();
            MilliSeconds = _watcher.ElapsedMilliseconds;
            Message = msg;
            Run = true;
            IsHealthy = healthy;
        }
    }
}
