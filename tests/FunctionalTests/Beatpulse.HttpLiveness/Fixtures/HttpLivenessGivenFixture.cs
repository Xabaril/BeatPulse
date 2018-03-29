using BeatPulse.Core.Http;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace FunctionalTests.Beatpulse.Http.Fixtures
{
    public class HttpLivenessGivenFixture
    {
        public const string DefaultTargetServerUrl  = "http://localhost:54000";

        public TestServer AServerWithHttpLiveness(HttpLivenessOptions options)
        {      
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .UseBeatPulse()
                .ConfigureServices(services =>
                {
                    services.AddBeatPulse(context =>
                    {
                        context.Add(new HttpLiveness(options));
                    });
                });

            return new TestServer(webHostBuilder);
        }

        public IWebHost ATargetServerWithConfiguredResponse(RequestDelegate @delegate, string url = DefaultTargetServerUrl)
        {
            
            var webhost = new WebHostBuilder()
                .UseUrls(new[] {url})               
                .UseKestrel()
                .Configure( app => {
                    app.Run(@delegate);
                })
                .Build();           

            return webhost;
        }

      
    }
}
