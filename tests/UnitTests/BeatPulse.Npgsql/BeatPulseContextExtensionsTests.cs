using BeatPulse.Core;
using BeatPulse.NpgSql;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Base;
using Xunit;

namespace BeatPulse.Npgsql
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_npgsql_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddNpgSql("the-npg-sql-connection-string");
                    });
                });

            var beatPulseContext = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContext.Should()
                .ContainsLiveness(nameof(NpgSqlLiveness));
        }
    }
}
