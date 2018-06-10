using System;

namespace BeatPulse.Core
{
    public class BeatPulseLivenessInstanceRegistration 
        : IBeatPulseLivenessRegistration
    {
        private readonly IBeatPulseLiveness _instance;

        public string Name { get; }
        public string Path { get; }

        public BeatPulseLivenessInstanceRegistration(IBeatPulseLiveness liveness, string name, string path = null)
        {
            _instance = liveness ?? throw new ArgumentNullException(nameof(liveness));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? name;
        }

        public IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider serviceProvider) => _instance;
    }
}
