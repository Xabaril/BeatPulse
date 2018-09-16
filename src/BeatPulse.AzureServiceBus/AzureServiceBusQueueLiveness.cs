using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.AzureServiceBus
{
    public class AzureServiceBusQueueLiveness : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _queueName;
        private const string TEST_MESSAGE = "BeatpulseTest";

        public AzureServiceBusQueueLiveness(string connectionString, string queueName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var queueClient = new QueueClient(_connectionString, 
                    _queueName, 
                    ReceiveMode.PeekLock);

                var scheduledMessageId = await queueClient.ScheduleMessageAsync(
                    new Message(Encoding.UTF8.GetBytes(TEST_MESSAGE)),
                    new DateTimeOffset(DateTime.UtcNow).AddHours(2));

                await queueClient.CancelScheduledMessageAsync(scheduledMessageId);

                return HealthCheckResult.Passed();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Failed(exception: ex);
            }
        }
    }
}
