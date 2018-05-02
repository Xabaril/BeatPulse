using System;
using System.Collections.Generic;

namespace BeatPulse.Core
{
    public sealed class BeatPulseContext
    {
        private readonly Dictionary<string, IBeatPulseLiveness> _activeLiveness = new Dictionary<string, IBeatPulseLiveness>();
        private readonly Dictionary<string, IBeatPulseTracker> _activeTrackers = new Dictionary<string, IBeatPulseTracker>();

        public BeatPulseContext Add(IBeatPulseLiveness liveness)
        {
            if (liveness == null)
            {
                throw new ArgumentNullException(nameof(liveness));
            }

            var defaultPath = liveness.DefaultPath;

            if (!string.IsNullOrEmpty(defaultPath))
            {
                if (!_activeLiveness.ContainsKey(defaultPath))
                {
                    _activeLiveness.Add(defaultPath, liveness);
                }
                else
                {
                    throw new InvalidOperationException($"The path {defaultPath} is already configured.");
                }

                return this;
            }
            else
            {
                throw new InvalidOperationException("The global path is automatically used for beat pulse.");
            }
        }

        public BeatPulseContext AddTracker(IBeatPulseTracker tracker)
        {
            var trackerName = nameof(tracker);
            if (tracker == null)
            {
                throw new ArgumentNullException(trackerName);
            }

            if (!_activeTrackers.ContainsKey(trackerName))
            {
                _activeTrackers.Add(trackerName, tracker);
            }
            else
            {
                throw new InvalidOperationException($"The tracker {trackerName} is already registered");
            }

            return this;

        }

        internal IBeatPulseLiveness FindLiveness(string path)
        {
            _activeLiveness.TryGetValue(path, out IBeatPulseLiveness check);

            return check;
        }

        internal IEnumerable<IBeatPulseLiveness> AllLiveness
        {
            get
            {
                return _activeLiveness.Values;
            }
        }

        internal IEnumerable<IBeatPulseTracker> AllTrackers
        {
            get
            {
                return _activeTrackers.Values;

            }
        }
    }
}
