using BeatPulse;
using BeatPulse.Core;
using BeatPulse.Core.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace HttpApi_Authentication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Registers an authorization filter that will validate the request 
            // when api-key parameter is present in the query string with api-key-secret value

            services.AddSingleton<IBeatPulseAuthenticationFilter>
                     (new ApiKeyAuthenticationFilter("api-key-secret"));

            // Registers an authorization filter that will validate the request
            // from local 

            services.AddSingleton<IBeatPulseAuthenticationFilter>(new LocalAuthenticationFilter());

            // Registers an authorization filter that will validate the request 
            // when the header "header1" is sent with value "value1"

            services.AddSingleton<IBeatPulseAuthenticationFilter>
                    (new HeaderValueAuthenticationFilter("header1", "value1"));

            services.AddBeatPulse(setup =>
            {
                //add sql server health check
                setup.AddSqlServer("Server=.;Initial Catalog=master;Integrated Security=true");

                //add custom health check
                setup.AddLiveness(new ActionLiveness("custom", "customapi", (httpContext, cancellationToken) =>
                {
                    return Task.FromResult(("OK", true));
                }));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}
