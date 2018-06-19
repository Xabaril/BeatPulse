using Autofac;
using BeatPulse.Core;
using BeatPulse.WebApi.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin
{
    public static class AutofacExtensions
    {
        public static IBeatPulseContextBuilder RegisterBeatPulse(this ContainerBuilder builder, Action<IBeatPulseOptions> optionsSetup = null)
        {
            var options = new BeatPulseOptions();
            optionsSetup?.Invoke(options);
            builder.RegisterInstance(options);
            builder.RegisterType<BeatPulseOwinMiddleware>().SingleInstance();
            return new AutofacBeatPulseBuilder(builder);
        }

    }
}
