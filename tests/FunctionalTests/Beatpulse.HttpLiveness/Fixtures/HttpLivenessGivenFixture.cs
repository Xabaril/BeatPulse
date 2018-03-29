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
        public static string TargetServerUrl { get; } = "http://localhost:54000";

        public TestServer AServerWithHttpLiveness(HttpLivenessOptions options)
        {
            var urlSegments = options.Url.Split("/");

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

        public IWebHost ATargetServerWithConfiguredResponse(RequestDelegate @delegate, string url = "http://localhost:54000")
        {
            
            var webhost = new WebHostBuilder()
                .UseUrls(new[] { url ?? TargetServerUrl })               
                .UseKestrel()
                .Configure( app => {
                    app.Run(@delegate);
                })
                .Build();           

            return webhost;
        }

      
    }
}
