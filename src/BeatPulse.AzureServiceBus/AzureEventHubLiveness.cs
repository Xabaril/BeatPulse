using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.AzureServiceBus
{
    public class AzureEventHubLiveness : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly string _eventHubName;

        public string Name => nameof(AzureEventHubLiveness);

        public string DefaultPath => "azureeventhub";
        public AzureEventHubLiveness(string connectionString, string eventHubName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _eventHubName = eventHubName ?? throw new ArgumentNullException(nameof(eventHubName));
        }
        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {                
                var connectionStringBuilder = new EventHubsConnectionStringBuilder(_connectionString)
                {
                    EntityPath = _eventHubName
                };

                var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
                await eventHubClient.GetRuntimeInformationAsync();

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
