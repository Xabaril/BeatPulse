using BeatPulse;
using BeatPulse.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HttpApi_Basic
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
            services.AddCors();

            services.AddBeatPulse(setup =>
            {
                //add sql server liveness
                setup.AddSqlServer("Server=.;Initial Catalog=master;Integrated Security=true");

                //add custom liveness using factory method. Using factory method allows us to get services through IServiceProvider
                setup.AddLiveness("catapi", sp => new ActionLiveness("cat", "catapi", (httpContext, cancellationToken) =>
                {
                    var log = sp.GetRequiredService<ILogger<ActionLiveness>>();
                    log.LogInformation("Running ActionLiveness");

                    if ((DateTime.UtcNow.Minute & 1) == 1)
                    {
                        return Task.FromResult(("OK", true));
                    }
                    else
                    {
                        return Task.FromResult(("the cat api is broken!", false));
                    }
                }));
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
