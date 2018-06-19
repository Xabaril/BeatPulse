using Autofac;
using BeatPulse.Core;
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

        public static IContainer ConfigureServices(this IAppBuilder app, Action<ContainerBuilder> cbAction)
        {
            var builder = new ContainerBuilder();
            cbAction.Invoke(builder);
            return builder.Build();
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
