using BeatPulse.Core;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.AzureServiceBus
{
    public class AzureServiceBusQueueLiveness : IBeatPulseLiveness
    {
        private const string TEST_MESSAGE = "BeatpulseTest";

        private readonly string _connectionString;
        private readonly string _queueName;
        private readonly ILogger<AzureServiceBusQueueLiveness> _logger;

        public AzureServiceBusQueueLiveness(string connectionString, string queueName, ILogger<AzureServiceBusQueueLiveness> logger = null)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _logger = logger;
        }

        public async Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(AzureServiceBusQueueLiveness)} is checking the Azure Event Hub.");

                var queueClient = new QueueClient(_connectionString, 
                    _queueName, 
                    ReceiveMode.PeekLock);

                var scheduledMessageId = await queueClient.ScheduleMessageAsync(
                    new Message(Encoding.UTF8.GetBytes(TEST_MESSAGE)),
                    new DateTimeOffset(DateTime.UtcNow).AddHours(2));

                await queueClient.CancelScheduledMessageAsync(scheduledMessageId);

                _logger?.LogInformation($"The {nameof(AzureServiceBusQueueLiveness)} check success.");

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(AzureServiceBusQueueLiveness)} check fail for {_connectionString} with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
