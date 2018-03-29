using BeatPulse.Core.Http;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionalTests.Beatpulse.Http.Fixtures
{
    public class HttpLivenessGivenFixture
    {       
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
    }
}
