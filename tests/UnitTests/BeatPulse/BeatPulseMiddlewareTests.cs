using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnitTests.Base;
using Xunit;

namespace BeatPulse
{
    [Collection(DefaultServerCollectionFixture.Default)]
    public class beat_pulse_middleware_should
    {
        
        private readonly DefaultServerFixture _fixture;

        public beat_pulse_middleware_should(DefaultServerFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task response_http_status_ok_when_beat_pulse_service_is_healthy()
        {
            var response = await _fixture.Server
                .CreateClient()
                .GetAsync(BeatPulseKeys.DefaultBeatPulsePath);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task response_http_status_serviceunavailable_when_beat_pulse_service_is_not_healthy()
        {
            var response = await _fixture.Server
               .CreateClient()
               .GetAsync(BeatPulseKeys.DefaultBeatPulsePath);

            response.StatusCode
                .Should()
                .Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task continue_chain_for_non_beat_pulse_requests()
        {
            var response = await _fixture.Server
                .CreateClient()
                .GetAsync("any-non-beatpulse-path");

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");
        }

        [Fact]
        public async Task continue_chain_for_not_valid_beat_pulse_http_verbs()
        {
            var response = await _fixture.Server
                .CreateClient()
                .PostAsync(BeatPulseKeys.DefaultBeatPulsePath,new StringContent(string.Empty));

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");

            response = await _fixture.Server
                .CreateClient()
                .PutAsync(BeatPulseKeys.DefaultBeatPulsePath, new StringContent(string.Empty));

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");

            response = await _fixture.Server
                .CreateClient()
                .DeleteAsync(BeatPulseKeys.DefaultBeatPulsePath);

            response.EnsureSuccessStatusCode();

            (await response.Content.ReadAsStringAsync()).Should()
                .Be("latest-midleware");
        }
    }
}
