using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace BeatPulse.Core
{
    public class BeatPulseContext
    {
        Dictionary<string, IBeatPulseHealthCheck> _healthCheckers = new Dictionary<string, IBeatPulseHealthCheck>();

        public BeatPulseContext Add(IBeatPulseHealthCheck check)
        {
            if (check == null)
            {
                throw new ArgumentNullException(nameof(check));
            }

            var defaultPath = check.HealthCheckDefaultPath;

            if (!string.IsNullOrEmpty(defaultPath))
            {
                if (!_healthCheckers.ContainsKey(defaultPath))
                {
                    _healthCheckers.Add(defaultPath, check);
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

        public IBeatPulseHealthCheck Find(string path)
        {
            _healthCheckers.TryGetValue(path, out IBeatPulseHealthCheck check);

            return check;
        }

        public IEnumerable<IBeatPulseHealthCheck> All
        {
            get
            {
                return _healthCheckers.Values;
            }
        }
    }
}
