using System;
using System.Collections.Generic;

namespace BeatPulse.Core
{
    public sealed class BeatPulseContext
    {
        private readonly Dictionary<string, IBeatPulseLiveness> _activeLiveness = new Dictionary<string, IBeatPulseLiveness>();

        public BeatPulseContext Add(IBeatPulseLiveness liveness)
        {
            if (liveness == null)
            {
                throw new ArgumentNullException(nameof(liveness));
            }

            var path = liveness.Path;

            if (!string.IsNullOrEmpty(path))
            {
                if (!_activeLiveness.ContainsKey(path))
                {
                    _activeLiveness.Add(path, liveness);
                }
                else
                {
                    throw new InvalidOperationException($"The path {path} is already configured.");
                }

                return this;
            }
            else
            {
                throw new InvalidOperationException("The global path is automatically used for beat pulse.");
            }
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
    }
}
