using BeatPulse.Core;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BeatPulse.Core
{
    public class beatpulsecontext_should
    {
        [Fact]
        public void register_multiple_liveness_with_the_sample_path()
        {
            var context = new BeatPulseContext();

            var options = new BeatPulseLivenessRegistrationOptions("name1");
            options.UsePath("path");
            options.UseLiveness(new ActionLiveness((cancellationToken) => Task.FromResult(("ok", true))));

            context.AddLiveness(options.CreateRegistration());

            options = new BeatPulseLivenessRegistrationOptions("name2");
            options.UsePath("path");
            options.UseLiveness(new ActionLiveness((cancellationToken) => Task.FromResult(("ok", true))));

            context.AddLiveness(options.CreateRegistration());

            context.GetAllLiveness("path")
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public void register_multiple_liveness_with_the_same_name_and_path()
        {
            var context = new BeatPulseContext();

            var options = new BeatPulseLivenessRegistrationOptions("name");
            options.UsePath("path");
            options.UseLiveness(new ActionLiveness((cancellationToken) => Task.FromResult(("ok", true))));

            context.AddLiveness(options.CreateRegistration());

            options = new BeatPulseLivenessRegistrationOptions("name");
            options.UsePath("path");
            options.UseLiveness(new ActionLiveness((cancellationToken) => Task.FromResult(("ok", true))));

            context.AddLiveness(options.CreateRegistration());

            context.GetAllLiveness("path")
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public void throw_if_try_to_register_a_default_system_path()
        {
            var context = new BeatPulseContext();

            var options = new BeatPulseLivenessRegistrationOptions("name1");
            options.UsePath(string.Empty);
            options.UseLiveness(new ActionLiveness((cancellationToken) => Task.FromResult(("ok", true))));

            Assert.Throws<InvalidOperationException>(() =>
           {
               context.AddLiveness(options.CreateRegistration());
           });
        }
    }
}
