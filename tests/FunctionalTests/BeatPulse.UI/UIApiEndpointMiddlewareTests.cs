using BeatPulse;
using BeatPulse.UI;
using BeatPulse.UI.Core;
using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net.Http.Headers;

namespace FunctionalTests.BeatPulse.UI
{
    public class UIApiEndpointMiddlewareTests
    {
        private string defaultUIPath = "/beatpulse-ui";
        private string defaultApiPath = "/beatpulse-api";

        [Fact]
        public async Task UI_dashboard_should_be_authorized_when_filters_success()
        {
            var webHostBuilder = new WebHostBuilder()
              .UseStartup<DefaultStartup>()
              .UseBeatPulse()
              .ConfigureServices(services =>
              {
                  services.AddBeatPulse().AddBeatPulseUI();
                  services.AddSingleton<IUIAuthorizationFilter, SuccessUIAuthorizationFilter>();
                  services.AddSingleton<IUIAuthorizationFilter>
                  (sp => new ApiKeyQueryUIAuthorizationFilter("secretkey"));

              }).Configure(app => app.UseBeatPulseUI());

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest($"{defaultUIPath}?apikey=secretkey").GetAsync();           
            
            response.Headers.GetValues("Location").FirstOrDefault().Should().Be($"{defaultUIPath}/index.html");
            response.StatusCode.Should().Be(StatusCodes.Status301MovedPermanently);
        }

        [Fact]
        public async Task UI_dashboard_should_be_unauthorized_when_a_filter_does_not_accept()
        {
            var webHostBuilder = new WebHostBuilder()
              .UseStartup<DefaultStartup>()
              .UseBeatPulse()
              .ConfigureServices(services =>
              {
                  services.AddBeatPulse().AddBeatPulseUI();
                  services.AddSingleton<IUIAuthorizationFilter>
                  (sp => new ApiKeyQueryUIAuthorizationFilter("secretkey"));
              }).Configure(app => app.UseBeatPulseUI()); ;

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"{defaultUIPath}").GetAsync();
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task UI_api_should_be_authorized_when_filters_success()
        {
            var webHostBuilder = new WebHostBuilder()
              .UseStartup<DefaultStartup>()
              .UseBeatPulse()
              .ConfigureServices(services =>
              {
                  services.AddBeatPulse().AddBeatPulseUI();
                  services.AddSingleton<IUIAuthorizationFilter, SuccessUIAuthorizationFilter>();
                  services.AddSingleton<IUIAuthorizationFilter>
                  (sp => new ApiKeyQueryUIAuthorizationFilter("secretkey"));

              }).Configure(app => app.UseBeatPulseUI());

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest($"{defaultApiPath}?apikey=secretkey").GetAsync();


            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType.Should().BeEquivalentTo(MediaTypeHeaderValue.Parse("application/json"));
        }

        [Fact]
        public async Task UI_api_should_be_unauthorized_when_filters_success()
        {
            var webHostBuilder = new WebHostBuilder()
              .UseStartup<DefaultStartup>()
              .UseBeatPulse()
              .ConfigureServices(services =>
              {
                  services.AddBeatPulse().AddBeatPulseUI();                  
                  services.AddSingleton<IUIAuthorizationFilter>
                  (sp => new ApiKeyQueryUIAuthorizationFilter("secretkey"));

              }).Configure(app => app.UseBeatPulseUI());

            var server = new TestServer(webHostBuilder);
            var response = await server.CreateRequest($"{defaultApiPath}").GetAsync();


            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }
    }


}
