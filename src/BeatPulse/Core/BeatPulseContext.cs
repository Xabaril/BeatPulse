using System;
using System.Collections.Generic;

namespace BeatPulse.Core
{
    public class BeatPulseContext
    {
        private readonly Dictionary<string, IBeatPulseLiveness> _activeLiveness = new Dictionary<string, IBeatPulseLiveness>();

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

        public IBeatPulseLiveness FindLiveness(string path)
        {
            _activeLiveness.TryGetValue(path, out IBeatPulseLiveness check);

            return check;
        }

        public IEnumerable<IBeatPulseLiveness> AllLiveness
        {
            get
            {
                return _activeLiveness.Values;
            }
        }
    }
}
