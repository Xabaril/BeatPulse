using BeatPulse.Core.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BeatPulse.Core.Authentication
{
    public class local_authentication_filter_should
    {
        [Fact]
        public async Task return_valid_if_request_is_from_remote_loopback()
        {
            var filter = new LocalAuthenticationFilter();
            var context = GetContextWithRequest("/path", remoteIpAddress:"::1", "127.0.1.100");

            (await filter.IsValid(context))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task return_valid_if_remote_and_local_are_equal()
        {
            var filter = new LocalAuthenticationFilter();
            var context = GetContextWithRequest("/path", remoteIpAddress: "127.0.1.100", "127.0.1.100");

            (await filter.IsValid(context))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task return_invalid_if_remote_and_local_are_not_equal()
        {
            var filter = new LocalAuthenticationFilter();
            var context = GetContextWithRequest("/path", remoteIpAddress: "25.36.78.151", "127.0.1.100");

            (await filter.IsValid(context))
                .Should()
                .BeFalse();
        }

        HttpContext GetContextWithRequest(string requestPath,string remoteIpAddress,string localIpAddress)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = requestPath;
            context.Connection.RemoteIpAddress = IPAddress.Parse(remoteIpAddress);
            context.Connection.LocalIpAddress = IPAddress.Parse(localIpAddress);
            return context;
        }
    }
}
