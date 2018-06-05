using BeatPulse;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace UnitTests.BeatPulse
{
    public class httpcontext_extensions_should
    {
        [Fact]
        public void return_true_if_request_is_beaptulse_request()
        {
            var options = new BeatPulseOptions();
            
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/hc"; 

            httpContext.IsBeatPulseRequest(options)
                .Should().BeTrue();

            options.SetAlternatePath("health");
            
            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/health";

            httpContext.IsBeatPulseRequest(options)
                .Should().BeTrue();
        }

        [Fact]
        public void return_the_beatpulse_path()
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

            options.SetAlternatePath("health");

            httpContext.Request.Path = "/health";

            httpContext.GetBeatPulseRequestPath(options)
                .Should().BeEquivalentTo(string.Empty);

            httpContext.Request.Path = "/health/sqlserver";

            httpContext.GetBeatPulseRequestPath(options)
                .Should().BeEquivalentTo("sqlserver");
        }
    }
}
