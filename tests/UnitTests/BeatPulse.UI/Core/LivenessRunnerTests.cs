using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BeatPulse.UI.Core
{
    public class liveness_runner_should
    {
        [Fact]
        public async Task notify_failures_if_liveness_is_down()
        {
            var notifier = new MemoryNotifier();
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var tokenSource = new CancellationTokenSource();
            var settings = Options.Create(new BeatPulseSettings());

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://www.google.es/healthlivenesspath", "Google Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runner = new LivenessRunner(context, notifier,settings,logger);

            await runner.Run(tokenSource.Token);

            notifier.ContainsFailureNotificationFor(livenessConfiguration.LivenessName)
                .Should().BeTrue();
        }

        [Fact]
        public async Task save_all_execution_histories()
        {
            var notifier = new MemoryNotifier();
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var tokenSource = new CancellationTokenSource();
            var settings = Options.Create(new BeatPulseSettings());

            var googleConfigurationLiveness = new LivenessConfigurationBuilder()
                .With("http://www.google.es/healthlivenesspath", "Google Liveness")
                .Build();

            var bingConfigurationLiveness = new LivenessConfigurationBuilder()
               .With("http://www.bing.es", "Bing Liveness")
               .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(googleConfigurationLiveness)
                .WithLiveness(bingConfigurationLiveness)
                .WithRandomDatabaseName()
                .Build();

            var runner = new LivenessRunner(context, notifier,settings, logger);

            await runner.Run(tokenSource.Token);

            var history = context.LivenessExecutionHistory
                .ToList();

            history.Count().Should().Be(2);

            history.Where(h => h.LivenessName == "Google Liveness")
                .Single()?.IsHealthy.Should().BeFalse();

            history.Where(h => h.LivenessName == "Bing Liveness")
                .Single()?.IsHealthy.Should().BeTrue();
        }

        [Fact]
        public async Task get_latest_get_latest_history_infirmation_for_specified_liveness()
        {
            var notifier = new MemoryNotifier();
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var tokenSource = new CancellationTokenSource();
            var settings = Options.Create(new BeatPulseSettings());
            var livenessName = "Bing Liveness";

            var bingConfigurationLiveness = new LivenessConfigurationBuilder()
              .With("http://www.bing.es", livenessName)
              .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(bingConfigurationLiveness)
                .WithRandomDatabaseName()
                .Build();

            var runner = new LivenessRunner(context, notifier,settings, logger);

            await runner.Run(tokenSource.Token);

            var history = await runner.GetLatestRun(livenessName, CancellationToken.None);

            history.Where(h => h.LivenessName == livenessName)
                .Single()?.IsHealthy.Should().BeTrue();
        }

        [Fact]
        public async Task get_all_livenes_configuration()
        {
            var notifier = new MemoryNotifier();
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var tokenSource = new CancellationTokenSource();
            var settings = Options.Create(new BeatPulseSettings());
            var livenessName = "Bing Liveness";

            var bingConfigurationLiveness = new LivenessConfigurationBuilder()
              .With("http://www.bing.es", livenessName)
              .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(bingConfigurationLiveness)
                .WithRandomDatabaseName()
                .Build();

            var runner = new LivenessRunner(context, notifier,settings, logger);

            var liveness =  await runner.GetLiveness(CancellationToken.None);

            liveness.Count.Should().Be(1);

            liveness.Single()
                .LivenessName.Should().Be(livenessName);
        }

        class MemoryNotifier : ILivenessFailureNotifier
        {
            Dictionary<string, string> _notifications;

            public MemoryNotifier()
            {
                _notifications = new Dictionary<string, string>();
            }

            public Task NotifyFailure(string name,string content)
            {
                if (!_notifications.ContainsKey(name))
                {
                    _notifications.Add(name, content);
                }

                return Task.CompletedTask;
            }

            public bool ContainsFailureNotificationFor(string name)
            {
                return _notifications.ContainsKey(name);
            }
        }
    }
}
