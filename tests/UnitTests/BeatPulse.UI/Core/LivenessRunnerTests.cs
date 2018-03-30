using BeatPulse.UI.Core;
using BeatPulse.UI.Core.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.BeatPulse.UI.Core
{
    public class liveness_runner_should
    {
        [Fact]
        public async Task execute_notify_failures_if_liveness_is_off()
        {
            var notifier = new TestNotifier();
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var tokenSource = new CancellationTokenSource();

            var context = new LivenessContextBuilder()
                .WithLiveness(new LivenessConfiguration()
                {
                    LivenessUri = "http://www.bing.es/health",
                    WebHookNotificationUri = "http://testwebhook.com"
                }).WithDatabaseName(nameof(execute_notify_failures_if_liveness_is_off)).Build();

            var runner = new LivenessRunner(context, notifier, logger);

            await runner.Run(tokenSource.Token);

            notifier.Notified
                .Should().BeTrue();
        }

        class TestNotifier : ILivenessFailureNotifier
        {
            public bool Notified { get; private set; }

            public TestNotifier()
            {
                Notified = false;
            }

            public Task NotifyFailure(string url,string content)
            {
                Notified = true;

                return Task.CompletedTask;
            }
        }
    }
}
