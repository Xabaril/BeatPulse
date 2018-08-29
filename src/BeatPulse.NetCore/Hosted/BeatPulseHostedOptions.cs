using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Hosted
{

    public interface IBeatPulseHostedOptions
    {
        IBeatPulseHostedOptions ConfigurePath(string path);
        IBeatPulseHostedOptions ConfigurePort(int port);
    }

    public class BeatPulseHostedOptions : IBeatPulseHostedOptions
    {
        private const int BEATPULSE_DEFAULT_PORT = 80;
        internal Type EndpointManagerType { get; private set; }
        internal BeatPulseContext  Context { get; private set; }

        private string _path;
        private int _port;

        public BeatPulseHostedOptions()
        {
            EndpointManagerType = typeof(BeatPulseWebServer);
            Context = new BeatPulseContext();
            _path = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
            _port = BEATPULSE_DEFAULT_PORT;
        }

        internal BeatPulseOptions BuildDefaultOptions()
        {
            var opt = new BeatPulseOptions();
            opt.ConfigurePath(_path);
            return opt;
        }


        public BeatPulseHostedOptions Configure(Action<IBeatPulseHostedOptions> configureAction)
        {
            configureAction?.Invoke(this);
            return this;
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

        IBeatPulseHostedOptions IBeatPulseHostedOptions.ConfigurePath(string path)
        {
            _path = path ?? throw new ArgumentNullException(path);
            return this;
        }

        IBeatPulseHostedOptions IBeatPulseHostedOptions.ConfigurePort(int port)
        {
            _port = port;
            return this;
        }
    }
}
