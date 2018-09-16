using System;
using BeatPulse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulseTrackers
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

            services.AddBeatPulse()

                //
                //configure a sample ad-hoc liveness
                //
                .AddDelegateCheck("catapi", failureStatus: null, tags: new[] { "catapi", }, () =>
                {
                    if (DateTime.Now.Minute == 20)
                    {
                        return HealthCheckResult.Failed("Service is down!");
                    }

                    return HealthCheckResult.Passed("OK");
                })

                //
                //add trackers
                //
                .AddApplicationInsightsTracker();

                //setup.AddPrometheusTracker(new Uri("http://localhost:9091"), new Dictionary<string, string>()
                //{
                //    {"MachineName",Environment.MachineName}
                //});

                //setup.AddStatusPageTracker(opt =>
                //{
                //    opt.PageId = "your-page-id";
                //    opt.ComponentId = "your-component-id";
                //    opt.ApiKey = "your-api.key";
                //    opt.IncidentName = "BeatPulse mark this component as outage";
            //});

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
