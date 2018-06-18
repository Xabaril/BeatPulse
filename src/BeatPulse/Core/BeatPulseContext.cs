using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatPulse.Core
{
    public sealed class BeatPulseContext
    {
        private readonly Dictionary<string, List<IBeatPulseLivenessRegistration>> _registeredLiveness
            = new Dictionary<string, List<IBeatPulseLivenessRegistration>>();

        private readonly List<IBeatPulseTrackerRegistration> _activeTrackers
            = new List<IBeatPulseTrackerRegistration>();

        private IServiceProvider _serviceProvider;

        public BeatPulseContext AddLiveness(IBeatPulseLivenessRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            var path = registration.Path;

            if (!string.IsNullOrEmpty(path))
            {
                if (!_registeredLiveness.ContainsKey(path))
                {
                    _registeredLiveness.Add(path, new List<IBeatPulseLivenessRegistration>() { registration });
                }
                else
                {
                    _registeredLiveness[path].Add(registration);
                }

                return this;
            }
            else
            {
                throw new InvalidOperationException("The global path is automatically used for beat pulse.");
            }
        }

        public BeatPulseContext AddTracker(IBeatPulseTrackerRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            _activeTrackers.Add(registration);

            return this;
        }

        public void UseServiceProvider(IServiceProvider sp)
        {
            _serviceProvider = sp;
        }

        public IBeatPulseLiveness CreateLivenessFromRegistration(IBeatPulseLivenessRegistration registration)
        {
            return registration.GetOrCreateLiveness(_serviceProvider);
        }

        public IEnumerable<IBeatPulseTracker> GetAllTrackers()
        {
            return _activeTrackers.Select(registration => registration.GetOrCreateTracker(_serviceProvider));
        }

        public IEnumerable<IBeatPulseLivenessRegistration> GetAllLiveness(string pathFilter = null)
        {
            if (String.IsNullOrEmpty(pathFilter))
            {
                return _registeredLiveness.Values.SelectMany(registration => registration);
            }
            else
            {
                return _registeredLiveness.TryGetValue(pathFilter, out List<IBeatPulseLivenessRegistration> check) ? check : Enumerable.Empty<IBeatPulseLivenessRegistration>();
            }
        }
    }
}
