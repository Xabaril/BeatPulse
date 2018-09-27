using BeatPulse;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace UnitTests.BeatPulse
{
    public class httpcontext_extensions_should
    {
        [Fact]
        public void evaluate_if_some_request_is_beatpulse_request()
        {
            var options = new BeatPulseOptions()
                .ConfigurePath("hc");
            
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/hc"; 

            httpContext.IsBeatPulseRequest(options)
                .Should().BeTrue();

            options.ConfigurePath("health");
            
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/health";

            httpContext.IsBeatPulseRequest(options)
                .Should().BeTrue();
        }
        [Fact]
        public void evaluate_if_some_request_is_beatpulse_request_when_port_is_configured()
        {
            var options = new BeatPulseOptions()
                .ConfigurePath("hc")
                .ConfigurePort(5000);

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/hc";
            httpContext.Connection.LocalPort = 5000;

            httpContext.IsBeatPulseRequest(options)
                .Should().BeTrue();

            httpContext.Connection.LocalPort = 6000;

            httpContext.IsBeatPulseRequest(options)
                .Should().BeFalse();
        }

        [Fact]
        public void evaluate_the_beatpulse_path()
        {
            var options = new BeatPulseOptions();

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/hc";

            httpContext.GetBeatPulseRequestPath(options)
                .Should().BeEquivalentTo(string.Empty);

            httpContext.Request.Path = "/hc/sqlserver";

            httpContext.GetBeatPulseRequestPath(options)
                .Should().BeEquivalentTo("sqlserver");

            options.ConfigurePath("health");

            httpContext.Request.Path = "/health";

            httpContext.GetBeatPulseRequestPath(options)
                .Should().BeEquivalentTo(string.Empty);

            httpContext.Request.Path = "/health/sqlserver";

            httpContext.GetBeatPulseRequestPath(options)
                .Should().BeEquivalentTo("sqlserver");
        }
    }
}
