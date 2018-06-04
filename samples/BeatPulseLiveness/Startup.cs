using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BeatPulse;
using BeatPulse.Core;
using Microsoft.Extensions.Logging;

namespace BeatPulseLiveness
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddBeatPulse(setup =>
            {       
                    //
                    //add existing liveness packages
                    //

                setup.AddSqlServer("the-sql-server-connection-string");
                // or setup.AddXXXX() for all liveness packages on Nuget

                    //
                    //create simple ad-hoc liveness
                    //

                setup.AddLiveness("catapi", opt =>
                {
                    opt.UsePath("catapi");
                    opt.UseLiveness(new ActionLiveness((httpContext, cancellationToken) =>
                    {
                        return Task.FromResult(("OK", true));
                    }));
                });

                    //
                    //ceate ad-hoc liveness with dependency resolution
                    //

                setup.AddLiveness("catapi", opt =>
                {
                    opt.UsePath("catapi");
                    opt.UseFactory(sp => new ActionLiveness((http, token) =>
                    {
                        var logger = sp.GetRequiredService<ILogger<Startup>>();
                        logger.LogInformation("Logger is a dependency for this liveness");

                        return Task.FromResult(("ok", true));
                    }));
                });
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
