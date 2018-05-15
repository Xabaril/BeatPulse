using System;

namespace BeatPulse.Core
{
    public class BeatPulseInstanceRegistration : IBeatPulseLivenessRegistration
    {
        private readonly IBeatPulseLiveness _instance;

        public string Path => _instance.Path;

        public BeatPulseInstanceRegistration(IBeatPulseLiveness liveness)
        {
            _instance = liveness ?? throw new ArgumentNullException(nameof(liveness));
        }

        public IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider sp) => _instance;
    }
}
