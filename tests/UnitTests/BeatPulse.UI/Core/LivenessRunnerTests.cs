using BeatPulse.UI.Core;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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
        public async Task execute_notify_failures_if_liveness_is_down()
        {
            var notifier = new MemoryNotifier();
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var tokenSource = new CancellationTokenSource();

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://www.google.es/healthlivenesspath", "Google Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runner = new LivenessRunner(context, notifier, logger);

            await runner.Run(tokenSource.Token);

            notifier.ContainsFailureNotificationFor(livenessConfiguration.LivenessName)
                .Should().BeTrue();
        }

        [Fact]
        public async Task execute_save_all_execution_histories()
        {
            var notifier = new MemoryNotifier();
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var tokenSource = new CancellationTokenSource();

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

            var runner = new LivenessRunner(context, notifier, logger);

            await runner.Run(tokenSource.Token);

            var history = context.LivenessExecutionHistory
                .ToList();

            history.Count().Should().Be(2);

            history.Where(h => h.LivenessName == "Google Liveness")
                .Single()?.IsHealthy.Should().BeFalse();

            history.Where(h => h.LivenessName == "Bing Liveness")
                .Single()?.IsHealthy.Should().BeTrue();
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
