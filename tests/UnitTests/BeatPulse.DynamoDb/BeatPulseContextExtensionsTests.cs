using Amazon;
using BeatPulse;
using BeatPulse.Core;
using BeatPulse.DynamoDb;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Base;
using Xunit;

namespace UnitTests.DynamoDb
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
                        context.AddDynamoDb(opt =>
                        {
                            opt.AccessKey = nameof(opt.AccessKey);
                            opt.RegionEndpoint = RegionEndpoint.APNortheast1;
                            opt.SecretKey = nameof(opt.SecretKey);
                        });
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.Should()
                .ContainsLiveness(nameof(DynamoDbLiveness));
        }
    }
}
