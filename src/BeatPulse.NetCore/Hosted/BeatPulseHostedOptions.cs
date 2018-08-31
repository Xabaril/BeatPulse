using BeatPulse.Core;
using System;
using System.Threading.Tasks;

namespace BeatPulse.Hosted
{

    public interface IBeatPulseHostedOptions
    {
        IBeatPulseHostedOptions ConfigurePath(string path);
        IBeatPulseHostedOptions ConfigurePort(int port);
        IBeatPulseHostedOptions ConfigureDetailedOutput(bool detailedOutput = true);

        IBeatPulseHostedOptions UseEndpointManager<T>() where T : IHostedBeatPulseEndpoint;

        IBeatPulseHostedOptions UseOutputFormatter<T>() where T : IHostedBeatpulseOutputFormatter;
        IBeatPulseHostedOptions UseOutputFormatter<T>(Func<IServiceProvider, IHostedBeatpulseOutputFormatter> creator);

    }

    public class BeatPulseHostedOptions : IBeatPulseHostedOptions
    {
        private const int BEATPULSE_DEFAULT_PORT = 8080;
        internal Type EndpointManagerType { get; private set; }
        internal Type OutputFormatterType { get; private set; }
        internal Func<IServiceProvider, IHostedBeatpulseOutputFormatter> OutputFormatterCreator { get; private set; }
        internal BeatPulseContext Context { get; private set; }


        public int Port { get; private set; }
        public string Path { get; private set; }

        public bool DetailedOutput { get; private set; }

        public BeatPulseHostedOptions()
        {
            EndpointManagerType = typeof(BeatPulseWebServer);
            Context = new BeatPulseContext();

            Context.AddLiveness(BeatPulseKeys.BEATPULSE_SELF_NAME, opt =>
            {
                var selfLiveness = new ActionLiveness(
                    cancellationToken => Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true)));
                opt.UsePath(BeatPulseKeys.BEATPULSE_SELF_SEGMENT);
                opt.UseLiveness(selfLiveness);
            });

            Path = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
            Port = BEATPULSE_DEFAULT_PORT;
            OutputFormatterType = typeof(JsonOutputFormatter);
            OutputFormatterCreator = null;
            DetailedOutput = false;
        }

        internal BeatPulseOptions BuildBeatPulseOptions()
        {
            var opt = new BeatPulseOptions();
            opt.ConfigurePath(Path);
            opt.ConfigureDetailedOutput(DetailedOutput);
            return opt;
        }


        public BeatPulseHostedOptions Configure(Action<IBeatPulseHostedOptions> configureAction)
        {
            configureAction?.Invoke(this);
            return this;
        }

        public BeatPulseHostedOptions Setup(Action<BeatPulseContext> setupAction)
        {
            setupAction?.Invoke(Context);
            return this;
        }

        IBeatPulseHostedOptions IBeatPulseHostedOptions.ConfigurePath(string path)
        {
            Path = path ?? throw new ArgumentNullException(path);
            return this;
        }

        IBeatPulseHostedOptions IBeatPulseHostedOptions.ConfigurePort(int port)
        {
            Port = port;
            return this;
        }

        IBeatPulseHostedOptions IBeatPulseHostedOptions.ConfigureDetailedOutput(bool detailedOutput = true)
        {
            DetailedOutput = detailedOutput;
            return this;
        }

        IBeatPulseHostedOptions IBeatPulseHostedOptions.UseEndpointManager<T>()
        {
            EndpointManagerType = typeof(T);
            return this;
        }


        IBeatPulseHostedOptions IBeatPulseHostedOptions.UseOutputFormatter<T>()
        {
            OutputFormatterType = typeof(T);
            OutputFormatterCreator = null;
            return this;
        }

        IBeatPulseHostedOptions IBeatPulseHostedOptions.UseOutputFormatter<T>(Func<IServiceProvider, IHostedBeatpulseOutputFormatter> creator)
        {
            OutputFormatterType = typeof(T);
            OutputFormatterCreator = creator;
            return this;
        }
    }
}
