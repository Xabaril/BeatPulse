using System;
using System.Diagnostics;

namespace BeatPulse.Core
{
    public class LivenessResult
    {
        private readonly Stopwatch _watcher = new Stopwatch();

        public string Name { get; private set; }

        public string Message { get; private set; }

        public TimeSpan Elapsed {get; private set;}

        public bool Run { get; private set; }

        public string Path { get; private set; }

        public bool IsHealthy { get; private set; }

        public LivenessResult(string name,string path)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? throw new ArgumentNullException(nameof(path));
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

            Elapsed = _watcher.Elapsed;
            Run = true;
            Message = message;
            IsHealthy = healthy;
        }
    }
}
