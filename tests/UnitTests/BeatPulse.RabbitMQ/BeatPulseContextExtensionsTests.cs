using BeatPulse.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UnitTests.Base;
using Xunit;

namespace BeatPulse.RabbitMQ
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_rabbitmq_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddRabbitMQ("the-rabbitmq-options");
                    });
                });

            var beatPulseContex = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContex.GetAllLiveness("rabbitmq")
                .Where(hc => hc.Name == nameof(RabbitMQLiveness))
                .Should().HaveCount(1);

        }
    }
}
