using BeatPulse.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Base;
using Xunit;

namespace BeatPulse.MongoDb
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_mongodb_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddMongoDb("the-mongodb-connection-string");
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.Should()
                .ContainsLiveness(nameof(MongoDbLiveness));
        }
    }
}
