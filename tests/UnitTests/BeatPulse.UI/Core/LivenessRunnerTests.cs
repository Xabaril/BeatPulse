using BeatPulse.UI.Core.Builders;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BeatPulse.UI.Core
{
    public class liveness_runner_should
    {
        [Fact]
        public async Task notify_failures_if_liveness_is_down()
        {
            var notifier = new MemoryNotifier();
            
            var tokenSource = new CancellationTokenSource();

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://someserver-1/health", "Failing Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runnerBuilder = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.ServiceUnavailable)
                .WithDegradedMessageContent()
                .WithLivenessDb(context)
                .WithMinimunElapsedSecondsOnNotifications(0)
                .WithNotifier(notifier);

            var runner = runnerBuilder.Build();

            await runner.Run(tokenSource.Token);

            notifier.ContainsFailureNotificationFor(livenessConfiguration.LivenessName)
                .Should().BeTrue();
        }

        [Fact]
        public async Task dont_notify_failure_if_not_elapsed_configured_elapsed_seconds()
        {
            var notifier = new MemoryNotifier();

            var tokenSource = new CancellationTokenSource();

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://someserver-1/health", "Failing Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runnerBuilder = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.ServiceUnavailable)
                .WithDegradedMessageContent()
                .WithLivenessDb(context)
                .WithMinimunElapsedSecondsOnNotifications(100)
                .WithNotifier(notifier);

            var runner = runnerBuilder.Build();

            await runner.Run(tokenSource.Token);

            await runner.Run(tokenSource.Token);

            notifier.ContainsFailureNotificationFor(livenessConfiguration.LivenessName)
                .Should().BeTrue();

            notifier.NumberOfNotificationTimesFor(livenessConfiguration.LivenessName)
                .Should().Be(1);
        }

        [Fact]
        public async Task save_execution_history_if_is_new()
        {
            var notifier = new MemoryNotifier();

            var tokenSource = new CancellationTokenSource();

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://someserver-2/health", "Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runnerBuilder = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.ServiceUnavailable)
                .WithDegradedMessageContent()
                .WithLivenessDb(context)
                .WithNotifier(notifier);

            var runner = runnerBuilder.Build();

            await runner.Run(tokenSource.Token);

            var history = context.LivenessExecutionHistory
                .ToList();

            history.Count().Should().Be(1);

            history.Where(h => h.LivenessName == "Liveness")
                .Single()?.IsHealthy.Should().BeFalse();
        }

        [Fact]
        public async Task get_latest_get_single_information_history_for_specified_liveness()
        {
            var notifier = new MemoryNotifier();

            var tokenSource = new CancellationTokenSource();

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://someserver-3/health", "Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runnerBuilder = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.OK)
                .WithHealthyMessageContent()
                .WithLivenessDb(context)
                .WithNotifier(notifier);

            var runner = runnerBuilder.Build();

            await runner.Run(tokenSource.Token);

            await runner.Run(tokenSource.Token);

            var history = context.LivenessExecutionHistory
                .ToList();

            history.Count().Should().Be(1);

            history.Where(h => h.LivenessName == "Liveness")
                .Single()?.IsHealthy.Should().BeTrue();
        }

        [Fact]
        public async Task save_execution_history_update_lastExecuted_if_history_exist_and_the_status_is_the_same()
        {
            var notifier = new MemoryNotifier();

            var tokenSource = new CancellationTokenSource();

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://someserver-4/health", "Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runnerBuilder = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.OK)
                .WithHealthyMessageContent()
                .WithLivenessDb(context)
                .WithNotifier(notifier);

            var runner = runnerBuilder.Build();

            await runner.Run(tokenSource.Token);

            var history1 = context.LivenessExecutionHistory
                .Single();

            await runner.Run(tokenSource.Token);

            var history2 = context.LivenessExecutionHistory
                .Single();

            history1.Id.Should().Be(history2.Id);
            history1.LivenessName.Should().Be(history2.LivenessName);
            history1.LivenessUri.Should().Be(history2.LivenessUri);
            history1.OnStateFrom.Should().Be(history2.OnStateFrom);
            history1.Status.Should().Be(history2.Status);
        }

        [Fact]
        public async Task save_execution_history_update_onStateFrom_if_history_exist_and_the_status_is_not_the_same()
        {
            var notifier = new MemoryNotifier();

            var tokenSource = new CancellationTokenSource();

            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://someserver-5/health", "Liveness")
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runnerBuilderOk = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.OK)
                .WithHealthyMessageContent()
                .WithLivenessDb(context)
                .WithNotifier(notifier);

            var runnerBuilderUnavailable = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.ServiceUnavailable)
                .WithDegradedMessageContent()
                .WithLivenessDb(context)
                .WithNotifier(notifier);

            var runnerOk = runnerBuilderOk.Build();

            await runnerOk.Run(tokenSource.Token);

            var history1 = context.LivenessExecutionHistory
                .Single();

            var status1 = history1.Status;
            var onStateFrom = history1.OnStateFrom;

            var runnerServiceUnavailable = runnerBuilderUnavailable.Build();

            await runnerServiceUnavailable.Run(tokenSource.Token);

            var history2 = context.LivenessExecutionHistory
                .Single();

            status1.Should().NotBe(history2.Status);
            onStateFrom.Should().NotBe(history2.OnStateFrom);
        }

        [Fact]
        public async Task get_all_livenes_configuration()
        {
            var notifier = new MemoryNotifier();
            var livenessName = "livenessName";
            var tokenSource = new CancellationTokenSource();


            var livenessConfiguration = new LivenessConfigurationBuilder()
                .With("http://someserver-6/health", livenessName)
                .Build();

            var context = new LivenessContextBuilder()
                .WithLiveness(livenessConfiguration)
                .WithRandomDatabaseName()
                .Build();

            var runnerBuilder = new LivenessRunnerBuilder()
                .WithHttpStatusCode(HttpStatusCode.OK)
                .WithHealthyMessageContent()
                .WithLivenessDb(context)
                .WithNotifier(notifier);

            var runner = runnerBuilder.Build();

            await runner.Run(tokenSource.Token);

            var liveness =  await runner.GetLiveness(CancellationToken.None);

            liveness.Count.Should().Be(1);

            liveness.Single()
                .LivenessName.Should().Be(livenessName);
        }

        class MemoryNotifier : ILivenessFailureNotifier
        {
            Dictionary<string, (int,string)> _notifications;

            public MemoryNotifier()
            {
                _notifications = new Dictionary<string,(int, string)>();
            }

            public Task NotifyFailure(string name,string content)
            {
                if (!_notifications.ContainsKey(name))
                {
                    _notifications.Add(name, (1, content));
                }
                else
                {
                    var (times, liveness) = _notifications[name];

                    _notifications[name] = (++times, liveness);
                }

                return Task.CompletedTask;
            }

            public bool ContainsFailureNotificationFor(string name)
            {
                return _notifications.ContainsKey(name);
            }

            public int NumberOfNotificationTimesFor(string name)
            {
                return _notifications[name].Item1;
            }
        }
    }

    
}
