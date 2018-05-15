using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatPulse.Core
{
    public sealed class BeatPulseContext
    {
        private readonly Dictionary<string, IBeatPulseLivenessRegistration> _registeredLiveness = new Dictionary<string, IBeatPulseLivenessRegistration>();

        private IServiceProvider _serviceProvider;

        internal void UseServiceProvider(IServiceProvider sp) => _serviceProvider = sp;

        public BeatPulseContext Add(IBeatPulseLivenessRegistration registration)
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


        internal IBeatPulseLiveness FindLiveness(string path)
        {
            _registeredLiveness.TryGetValue(path, out IBeatPulseLivenessRegistration check);

            return check?.GetOrCreateLiveness(_serviceProvider);
        }

        internal IEnumerable<IBeatPulseLiveness> AllLiveness
        {
            get
            {
                return _registeredLiveness.Values.Select(r => r.GetOrCreateLiveness(_serviceProvider));
            }
        }
    }
}
