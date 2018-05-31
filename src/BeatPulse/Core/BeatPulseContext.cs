using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatPulse.Core
{
    public sealed class BeatPulseContext
    {
        private readonly Dictionary<string, IBeatPulseLivenessRegistration> _registeredLiveness
            = new Dictionary<string, IBeatPulseLivenessRegistration>();

        private readonly Dictionary<string, IBeatPulseTrackerRegistration> _activeTrackers
            = new Dictionary<string, IBeatPulseTrackerRegistration>();

        private IServiceProvider _serviceProvider;

        internal void UseServiceProvider(IServiceProvider sp) => _serviceProvider = sp;

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

            var name = registration.Name;

            if (!_activeTrackers.ContainsKey(name))
            {
                _activeTrackers.Add(name, registration);
            }
            else
            {
                throw new InvalidOperationException($"The tracker {registration.Name} is already registered.");
            }

            return this;
        }

        internal IBeatPulseLiveness FindLiveness(string path)
        {
            _registeredLiveness.TryGetValue(path, out IBeatPulseLivenessRegistration check);

            return check?.GetOrCreateLiveness(_serviceProvider);
        }

        internal IBeatPulseLivenessRegistration FindLivenessRegistration(string path)
        {
            return _registeredLiveness.TryGetValue(path, out IBeatPulseLivenessRegistration check) ? check : null;
        }



        internal IEnumerable<IBeatPulseLiveness> AllLiveness
        {
            get
            {
                return _registeredLiveness.Values.Select(registration => registration.GetOrCreateLiveness(_serviceProvider));
            }
        }

        internal IBeatPulseLiveness GetLivenessFromRegistration(IBeatPulseLivenessRegistration registration) => registration.GetOrCreateLiveness(_serviceProvider);

        internal IEnumerable<IBeatPulseLivenessRegistration> AllLivenessRegistrations => _registeredLiveness.Values;

        internal IEnumerable<IBeatPulseTracker> AllTrackers
        {
            get
            {
                return _activeTrackers.Values.Select(registration => registration.GetOrCreateTracker(_serviceProvider));
            }
        }
    }
}
