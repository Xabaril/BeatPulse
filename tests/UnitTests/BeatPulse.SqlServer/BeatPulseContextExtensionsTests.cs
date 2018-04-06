using BeatPulse.Core;
using BeatPulse.SqlServer;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using UnitTests.Base;
using Xunit;

namespace BeatPulse.SqlServer
{
    public class beat_pulse_context_should
    {
        [Fact]
        public void register_sqlserver_liveness()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddSqlServer("the-sql-server-connection-string");
                    });
                });

            var beatPulseContex = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContex.AllLiveness
                .Where(hc => hc.GetType() == typeof(SqlServerLiveness))
                .Should().HaveCount(1);

        }

        [Fact]
        public void register_sqlserver_liveness_more_than_once_should_be_allowed()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseBeatPulse()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(svc =>
                {
                    svc.AddBeatPulse(context =>
                    {
                        context.AddSqlServer("the-sql-server-connection-string");
                        context.AddSqlServer("the-sql-server-connection-string-2");
                        context.AddSqlServer("the-sql-server-connection-string-3");
                    });
                });

            var beatPulseContex = new TestServer(webHostBuilder)
                .Host
                .Services
                .GetService<BeatPulseContext>();

            beatPulseContex.AllLiveness
                .Where(hc => hc.GetType() == typeof(SqlServerLiveness))
                .Should().HaveCount(3);

            beatPulseContex
                .AllLivenesOfType<SqlServerLiveness>()
                .First()
                .DefaultPath.Should().BeEquivalentTo("sqlserver");

            beatPulseContex
                .AllLivenesOfType<SqlServerLiveness>()
                .Last()
                .DefaultPath.Should().BeEquivalentTo("sqlserver-2");
        }


    }
}
