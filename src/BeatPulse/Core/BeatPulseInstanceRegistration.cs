using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public class BeatPulseInstanceRegistration : IBeatPulseLivenessRegistration
    {

        private readonly IBeatPulseLiveness _instance;

        public string Path => _instance.Path;
        public Guid Id { get; }

        public BeatPulseInstanceRegistration(IBeatPulseLiveness liveness)
        {
            _instance = liveness ?? throw new ArgumentNullException(nameof(liveness));
            Id = Guid.NewGuid();
        }

        public IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider sp) => _instance;
    }
}
