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

        /// <summary>
        /// Add a new liveness into the <see cref="BeatPulseContext"/>
        /// </summary>
        /// <param name="registration">The liveness registration to be added.</param>
        /// <returns><see cref="BeatPulseContext"/></returns>
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

        /// <summary>
        /// Add a new tracker into the <see cref="BeatPulseContext"/>
        /// </summary>
        /// <param name="registration">The liveness tracker registration to be added.</param>
        /// <returns><see cref="BeatPulseContext"/></returns>
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

        internal IEnumerable<IBeatPulseTracker> GetAllTrackers()
        {
            return _activeTrackers.Select(registration => registration.GetOrCreateTracker(_serviceProvider));
        }

        internal IEnumerable<IBeatPulseLivenessRegistration> GetAllLiveness(string pathFilter = null)
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
