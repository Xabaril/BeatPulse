using BeatPulse;
using BeatPulse.Core;
using BeatPulse.Hosted;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Hosting
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseBeatPulse(this IHostBuilder builder, Action<BeatPulseHostedOptions> setup = null)
        {
            var options = new BeatPulseHostedOptions();
            setup?.Invoke(options);
            builder.ConfigureHostConfiguration(cb => { });
            builder.ConfigureServices((hc, sc) =>
            {
                sc.AddSingleton<BeatPulseContext>(options.Context);
                sc.AddSingleton<IBeatPulseService, BeatPulseService>();
                sc.AddSingleton(typeof(IHostedBeatPulseEndpoint), options.EndpointManagerType);
                if (options.OutputFormatterCreator == null)
                {
                    sc.AddTransient(typeof(IHostedBeatpulseOutputFormatter), options.OutputFormatterType);
                }
                else
                {
                    sc.AddTransient<IHostedBeatpulseOutputFormatter>(options.OutputFormatterCreator);
                }
                sc.AddSingleton(options);
                sc.AddHostedService<BeatPulseHostedPipeline>();
            });


            return builder;
        }
    }
}
