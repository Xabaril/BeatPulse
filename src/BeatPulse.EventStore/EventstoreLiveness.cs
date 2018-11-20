using BeatPulse.Core;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.EventStore
{
    public class EventStoreLiveness : IBeatPulseLiveness
    {
        private readonly string _uri;
        private readonly string _login;
        private readonly string _password;
        private readonly ILogger<EventStoreLiveness> _logger;

        public EventStoreLiveness(string uri, string login, string password, ILogger<EventStoreLiveness> logger = null)
        {
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _login = login;
            _password = password;
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {

                _logger?.LogInformation($"{nameof(EventStoreLiveness)} is checking the EventStore host.");

                var eventStoreUri = new Uri(_uri);

                ConnectionSettings connectionSettings;

                if (string.IsNullOrEmpty(_login) || string.IsNullOrEmpty(_password))
                {
                    connectionSettings = ConnectionSettings.Create()
                        .KeepRetrying()
                        .Build();
                }
                else
                {
                    connectionSettings = ConnectionSettings.Create()
                        .KeepRetrying()
                        .SetDefaultUserCredentials(new UserCredentials(_login, _password))
                        .Build();
                }

                var connection = EventStoreConnection.Create(
                    connectionSettings,
                    new Uri(_uri),
                    "BeatPulse HealthCheck");

                await connection.ConnectAsync();

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(EventStoreLiveness)} check fail for Eventstore with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}