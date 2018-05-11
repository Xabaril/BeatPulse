using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;

namespace BeatPulse.AzureServiceBus
{
    public class AzureServiceBusTopicLiveness : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private const string TEST_MESSAGE = "BeatpulseTest"; 

        public string Name => nameof(AzureServiceBusTopicLiveness);
        public string Path { get; set; }

        public AzureServiceBusTopicLiveness(string connectionString, string topicName, string defaultPath)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _topicName = topicName ?? throw new ArgumentNullException(nameof(topicName));
            Path = defaultPath ?? throw new ArgumentNullException(nameof(defaultPath));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                var topicClient = new TopicClient(_connectionString, _topicName);
                var scheduledMessageId = await topicClient.ScheduleMessageAsync(
                                            new Message(Encoding.UTF8.GetBytes(TEST_MESSAGE)),
                                            new DateTimeOffset(DateTime.UtcNow).AddHours(2)
                                         );

                await topicClient.CancelScheduledMessageAsync(scheduledMessageId);

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
