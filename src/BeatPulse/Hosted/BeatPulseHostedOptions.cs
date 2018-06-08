using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Hosted
{
    public class BeatPulseHostedOptions
    {
        internal Type EndpointManagerType { get; private set; }
        internal BeatPulseContext  Context { get; private set; }

        public BeatPulseHostedOptions()
        {
            EndpointManagerType = typeof(BeatPulseWebServer);
            Context = new BeatPulseContext();
        }

        public BeatPulseHostedOptions UseCustomEndpointManager<T>() where T : IHostedBeatPulseEndpoint
        {
            EndpointManagerType = typeof(T);
            return this;
        }

        public BeatPulseHostedOptions Setup(Action<BeatPulseContext> setupAction)
        {
            setupAction?.Invoke(Context);
            return this;
        }
    }
}
