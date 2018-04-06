using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<TL> AllLivenesOfType<TL>()
            where TL : IBeatPulseLiveness =>
            _activeLiveness.Values.OfType<TL>();
        
    }
}
