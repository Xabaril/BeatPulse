using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac.Integration.WebApi;
using BeatPulse.Owin;
using Microsoft.Owin;
using BeatPulse;
using Owin;

[assembly: OwinStartup(typeof(BeatPulseWebApiOwin.Startup))]

namespace BeatPulseWebApiOwin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            var container = app.ConfigureServices(cb =>
            {
                cb.RegisterApiControllers(Assembly.GetExecutingAssembly());
                cb.RegisterBeatPulse(opt =>
                {
                    opt.ConfigurePath("/hc");
                })
                .With(opt => opt.AddSqlServer("d"));
            });

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseBeatPulse(container);
            app.UseWebApi(config);
        }
    }
}
