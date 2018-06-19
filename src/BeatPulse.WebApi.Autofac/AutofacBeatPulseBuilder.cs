using Autofac;
using BeatPulse.Core;
using BeatPulse.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.WebApi.Autofac
{
    class AutofacBeatPulseBuilder : IBeatPulseContextBuilder
    {
        private readonly ContainerBuilder _builder;
        public AutofacBeatPulseBuilder(ContainerBuilder builder)
        {
            _builder = builder;
        }
        public void With(Action<BeatPulseContext> contextSetup)
        {
            var beatPulseServiceBuilder = new BeatPulseServiceBuilder();
            var service = beatPulseServiceBuilder.Buid(ctx =>
            {
                contextSetup?.Invoke(ctx);
            });
            _builder.RegisterInstance(service).As<IBeatPulseService>();
        }

    }
}
