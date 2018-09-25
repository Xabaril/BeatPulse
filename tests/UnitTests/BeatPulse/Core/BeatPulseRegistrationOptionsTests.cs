using BeatPulse.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BeatPulse.Core
{
    public class beat_pulse_registration_options_should
    {
        [Fact]
        public void create_instance_liveness_registration_from_options()
        {
            var serviceCollection = new ServiceCollection();

            var options = new BeatPulseLivenessRegistrationOptions("defaultName");
            options.UseLiveness(new FakeLiveness());

            var registration = options.CreateRegistration();

            registration.Name.Should().BeEquivalentTo("defaultName");

            var liveness = registration.GetOrCreateLiveness(serviceCollection.BuildServiceProvider());

            liveness.GetType()
                .Should().Be(typeof(FakeLiveness));
        }

        [Fact]
        public void create_factory_liveness_registration_from_options()
        {
            var serviceCollection = new ServiceCollection();

            var options = new BeatPulseLivenessRegistrationOptions("defaultName");
            options.UseFactory(sp => new FakeLiveness());

            var registration = options.CreateRegistration();

            registration.Name.Should().BeEquivalentTo("defaultName");

            var liveness = registration.GetOrCreateLiveness(serviceCollection.BuildServiceProvider());

            liveness.GetType()
                .Should().Be(typeof(FakeLiveness));
        }

        [Fact]
        public void instance_before_factory_when_create_from_options()
        {
            var serviceCollection = new ServiceCollection();

            var liveness1 = new FakeLiveness();
            var liveness2 = new FakeLiveness();

            var options = new BeatPulseLivenessRegistrationOptions("defaultName");
            options.UseLiveness(liveness1);
            options.UseFactory(sp => liveness2);

            var registration = options.CreateRegistration();

            registration.Name.Should().BeEquivalentTo("defaultName");

            var liveness = registration.GetOrCreateLiveness(serviceCollection.BuildServiceProvider());

            liveness.GetType()
                .Should().Be(typeof(FakeLiveness));
            liveness.Equals(liveness1);
        }

        [Fact]
        public void throw_is_not_liveness_or_factory_is_used()
        {
            var options = new BeatPulseLivenessRegistrationOptions("defaultName");

            Assert.Throws<InvalidOperationException>(() =>
            {
                options.CreateRegistration();
            });
        }

        class FakeLiveness : IBeatPulseLiveness
        {
            public Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(("OK", true));
            }
        }
    }
}
