using BeatPulse.UI.Configuration;
using BeatPulse.UI.Core.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace BeatPulse.UI.Core.Builders
{
    class LivenessRunnerBuilder
    {
        HttpStatusCode _status;
        string _content;
        ILivenessFailureNotifier _notifier;
        LivenessDb _livenessDb;
        int _minimumElapsedNotificationSeconds = 0;

        public LivenessRunnerBuilder WithHttpStatusCode(HttpStatusCode httpStatus)
        {
            _status = httpStatus;

            return this;
        }

        public LivenessRunnerBuilder WithNotifier(ILivenessFailureNotifier notifier)
        {
            _notifier = notifier;

            return this;
        }

        public LivenessRunnerBuilder WithLivenessDb(LivenessDb livenessDb)
        {
            _livenessDb = livenessDb;

            return this;
        }

        public LivenessRunnerBuilder WithHealthyMessageContent()
        {
            _content = @"{\""Checks\"":[{\""Name\"":\""self\"",\""Message\"":\""OK\"",\""MilliSeconds\"":1,\""Run\"":true,\""Path\"":\""_self\"",\""IsHealthy\"":true},{\""Name\"":\""SqlServerLiveness\"",\""Message\"":\""OK\"",\""MilliSeconds\"":300,\""Run\"":true,\""Path\"":\""sqlserver\"",\""IsHealthy\"":true}],\""StartedAtUtc\"":\""2018-04-07T19:57:12.6794603Z\"",\""EndAtUtc\"":\""2018-04-07T19:57:13.4395631Z\""}";

            return this;
        }

        public LivenessRunnerBuilder WithDegradedMessageContent()
        {
            _content = @"{\""Checks\"":[{\""Name\"":\""self\"",\""Message\"":\""OK\"",\""MilliSeconds\"":1,\""Run\"":true,\""Path\"":\""_self\"",\""IsHealthy\"":true},{\""Name\"":\""SqlServerLiveness\"",\""Message\"":\""Service Unavailable\"",\""MilliSeconds\"":300,\""Run\"":true,\""Path\"":\""sqlserver\"",\""IsHealthy\"":false}],\""StartedAtUtc\"":\""2018-04-07T19:57:12.6794603Z\"",\""EndAtUtc\"":\""2018-04-07T19:57:13.4395631Z\""}";

            return this;
        }

        public LivenessRunnerBuilder WithMinimumElapsedSecondsOnNotifications(int elapsedSeconds)
        {
            _minimumElapsedNotificationSeconds = elapsedSeconds;

            return this;
        }

        public LivenessRunner Build()
        { 
            var logger = new Logger<LivenessRunner>(new LoggerFactory());
            var options = Options.Create(new BeatPulseSettings()
            {
                MinimumSecondsBetweenFailureNotifications = _minimumElapsedNotificationSeconds
            });

            return new FakeLivenessRunner(_status, _content, _livenessDb, _notifier,options, logger);
        }
    }
}
