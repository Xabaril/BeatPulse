using BeatPulse;
using BeatPulse.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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

                //setup.AddSqlServer("Server=.;Integrated Security=true;Initial Catalog=master");
                // or setup.AddXXXX() for all liveness packages on Nuget (mysql,sqlite,urlgroup,redis,idsvr,kafka,aws dynamo,azure storage and much more)
                // ie: setup.AddOracle("Data Source=localhost:49161/xe;User Id=system;Password=oracle");

                setup.AddUrlGroup(new Uri[] { new Uri("http://www.google.es")});

                setup.AddUrlGroup(opt =>
                {
                    opt.AddUri(new Uri("http://google.com"), uri =>
                    {
                        uri.UsePost()
                           .AddCustomHeader("X-Method-Override", "DELETE");
                    });
                }, "uri-group2", "UriLiveness2");

                //
                //create simple ad-hoc liveness
                //

                setup.AddLiveness("custom-liveness", opt =>
                {
                    opt.UsePath("custom-liveness");
                    opt.UseLiveness(new ActionLiveness((cancellationToken) =>
                    {
                        return Task.FromResult(
                            LivenessResult.Healthy());
                    }));
                });

                //
                //ceate ad-hoc liveness with dependency resolution
                //

                setup.AddLiveness("custom-liveness-with-dependency", opt =>
                {
                    opt.UsePath("custom-liveness-with-dependency");
                    opt.UseFactory(sp => new ActionLiveness((cancellationToken) =>
                    {
                        var logger = sp.GetRequiredService<ILogger<Startup>>();
                        logger.LogInformation("Logger is a dependency for this liveness");

                        return Task.FromResult(
                            LivenessResult.Healthy());
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
