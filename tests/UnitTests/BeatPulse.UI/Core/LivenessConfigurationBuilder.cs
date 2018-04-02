using BeatPulse.UI.Core.Data;
using System;

namespace UnitTests.BeatPulse.UI.Core
{
    class LivenessConfigurationBuilder
    {
        private string _livenessUri;
        private string _livenessName;

        public LivenessConfigurationBuilder With(string uri, string name)
        {
            _livenessUri = uri ?? throw new ArgumentNullException(nameof(uri));
            _livenessName = name ?? throw new ArgumentNullException(nameof(name));

            return this;
        }

        public LivenessConfiguration Build()
        {
            return new LivenessConfiguration()
            {
                LivenessName = _livenessName,
                LivenessUri = _livenessUri
            };
        }
    }
}
