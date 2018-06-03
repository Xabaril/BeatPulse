using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatPulse.Core
{
    public sealed class BeatPulseContext
    {
        private readonly Dictionary<string, IBeatPulseLivenessRegistration> _registeredLiveness
            = new Dictionary<string, IBeatPulseLivenessRegistration>();

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
                    _registeredLiveness.Add(path, registration);
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

        public BeatPulseContext AddTracker(IBeatPulseTrackerRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            _activeTrackers.Add(registration);

            return this;
        }

        internal void UseServiceProvider(IServiceProvider sp)
        {
            _serviceProvider = sp;
        }

        internal IBeatPulseLiveness CreateLivenessFromRegistration(IBeatPulseLivenessRegistration registration)
        {
            return registration.GetOrCreateLiveness(_serviceProvider);
        }

        internal IEnumerable<IBeatPulseTracker> AllTrackers
        {
            get
            {
                return _activeTrackers.Select(registration => registration.GetOrCreateTracker(_serviceProvider));
            }
        }

        internal IEnumerable<IBeatPulseLivenessRegistration> GetAllLivenessRegistrations()
        {
            return _registeredLiveness.Values;
        }

        internal IBeatPulseLivenessRegistration FindLivenessRegistration(string path)
        {
            return _registeredLiveness.TryGetValue(path, out IBeatPulseLivenessRegistration check) ? check : null;
        }
    }
}
