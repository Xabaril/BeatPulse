using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;

namespace BeatPulse.AzureServiceBus
{
    public class AzureServiceBusQueueLiveness : IBeatPulseLiveness
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private const string TEST_MESSAGE = "BeatpulseTest";

        public AzureServiceBusQueueLiveness(string connectionString, string queueName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                var queueClient = new QueueClient(_connectionString, _queueName, ReceiveMode.PeekLock);
                var scheduledMessageId = await queueClient.ScheduleMessageAsync(
                                            new Message(Encoding.UTF8.GetBytes(TEST_MESSAGE)),
                                            new DateTimeOffset(DateTime.UtcNow).AddHours(2)
                                        );

                await queueClient.CancelScheduledMessageAsync(scheduledMessageId);

                return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
            }
            catch (Exception ex)
            {
                var isDevelopment = livenessContext.IsDevelopment;
                var name = livenessContext.Name;
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
