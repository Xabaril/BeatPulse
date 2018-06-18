using BeatPulse;
using BeatPulse.Core;
using BeatPulse.Core.Authentication;
using BeatPulseLivenessAuthentication.Infrastructure.AuthenticationFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BeatPulseLivenessAuthentication
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

                //
                // Configure BeatPulse Authenthication filters
                //

            //api-key filter
            //services.AddSingleton<IBeatPulseAuthenticationFilter>
            //         (new ApiKeyAuthenticationFilter("api-key-secret"));

            //local filter
            //services.AddSingleton<IBeatPulseAuthenticationFilter>(
            //    new LocalAuthenticationFilter());

            //custom filter ( defined on this project ) 
            //services.AddSingleton<IBeatPulseAuthenticationFilter>
            //        (new HeaderValueAuthenticationFilter("header1", "value1"));


            services.AddBeatPulse(setup =>
            {
                //
                //create simple ad-hoc liveness
                //

                setup.AddLiveness("catapi", opt =>
                {
                    opt.UsePath("catapi");
                    opt.UseLiveness(new ActionLiveness(cancellationToken =>
                    {
                        return Task.FromResult(("OK", true));
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
