using BeatPulse.Core.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BeatPulse.Core.Authentication
{
    public class api_key_authentication_filter_should
    {
        [Fact]
        public async Task return_valid_if_the_query_string_contain_the_configured_value()
        {
            var apiKeyValue = "the-api-key-value";

            var filter = new ApiKeyAuthenticationFilter(apiKeyValue: apiKeyValue);
            var context = GetContextWithRequest("/path","?api-key=the-api-key-value");

            (await filter.IsValid(context))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task return_valid_if_the_query_string_contain_the_configured_value_in_different_case()
        {
            var apiKeyValue = "the-api-key-value";

            var filter = new ApiKeyAuthenticationFilter(apiKeyValue: apiKeyValue);
            var context = GetContextWithRequest("/path", "?api-key=THE-api-KEY-value");

            (await filter.IsValid(context))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task return_valid_if_the_query_string_contain_the_configured_value_on_any()
        {
            var apiKeyValue = "the-api-key-value";

            var filter = new ApiKeyAuthenticationFilter(apiKeyValue: apiKeyValue);
            var context = GetContextWithRequest("/path", "?api-key=THE-api-KEY-value&api-key=invalid-api-key");

            (await filter.IsValid(context))
                .Should()
                .BeTrue();
        }

        [Fact]
        public async Task return_no_valid_if_the_query_string_contain_the_configured_value()
        {
            var apiKeyValue = "the-api-key-value";

            var filter = new ApiKeyAuthenticationFilter(apiKeyValue: apiKeyValue);
            var context = GetContextWithRequest("/path", "?api-key=the-invalid-api-key");

            (await filter.IsValid(context))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void throw_if_apikey_value_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ApiKeyAuthenticationFilter(apiKeyValue: null));
        }

        [Fact]
        public void throw_if_apikey_name_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ApiKeyAuthenticationFilter(apiKeyValue: "some-value",apiKeyName:null));
        }

        HttpContext GetContextWithRequest(string requestPath,string query)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = requestPath;
            context.Request.QueryString = new QueryString(query);
            return context;
        }
    }
}
