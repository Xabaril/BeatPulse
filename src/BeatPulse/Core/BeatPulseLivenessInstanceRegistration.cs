using System;

namespace BeatPulse.Core
{
    public class BeatPulseLivenessInstanceRegistration 
        : IBeatPulseLivenessRegistration
    {
        private readonly IBeatPulseLiveness _instance;

        public string Path => _instance.Path;

        public BeatPulseLivenessInstanceRegistration(IBeatPulseLiveness liveness)
        {
            _instance = liveness ?? throw new ArgumentNullException(nameof(liveness));
        }

        public IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider serviceProvider) => _instance;
    }
}
