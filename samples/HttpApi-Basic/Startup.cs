using BeatPulse;
using BeatPulse.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

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
            services.AddBeatPulse(setup =>
            {
                //add sql server health check
                setup.AddSqlServer("Server=.;Initial Catalog=master;Integrated Security=true");

                //add custom health check
                setup.Add(new ActionLiveness("cat", "catapi", async (httpContext, cancellationToken) =>
                {
                    var httpClient = new HttpClient()
                    {
                        BaseAddress = new Uri("http://www.google.es")
                    };

                    var response = await httpClient.GetAsync(string.Empty, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        return ("OK", true);
                    }
                    else
                    {
                        return ("the cat api is broken!", false);
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
