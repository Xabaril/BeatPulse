using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;

namespace UnitTests.Base
{
    public class TestServerBuilder
    {
        string _pulsePath = "hc";

        public TestServerBuilder WithPulsePath(string path)
        {
            _pulsePath = path ?? throw new ArgumentNullException(nameof(path));

            return this;
        }

        public TestServer Build()
        {
            var hostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse(_pulsePath);

            return new TestServer(hostBuilder);
        }

        class DefaultStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {

            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("latest-midleware");
                });
            }
        }
    }
}
