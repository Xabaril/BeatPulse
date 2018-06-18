using Autofac;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin
{
    public static class IAppBuilderExtensions
    {
        public static void AddBeatPulse(ContainerBuilder builder)
        {
            builder.RegisterType<BeatPulseOwinMiddleware>();
        }

        public static IAppBuilder UseBeatPulse(this IAppBuilder app, IContainer container)
        {
            app.UseAutofacLifetimeScopeInjector(container);
            app.UseMiddlewareFromContainer<BeatPulseOwinMiddleware>();
            return app;
        }

        public static IAppBuilder UseBeatPulse(this IAppBuilder app)
        {
            app.UseMiddlewareFromContainer<BeatPulseOwinMiddleware>();
            return app;
        }
    }
}
