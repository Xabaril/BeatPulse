using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.RabbitMQ
{
    public class RabbitMQLiveness : IBeatPulseLiveness
    {
        private readonly string _rabbitMqConnectionString;
        private readonly ILogger<RabbitMQLiveness> _logger;

        public RabbitMQLiveness(string rabbitMqConnectionString, ILogger<RabbitMQLiveness> logger = null)
        {
            _rabbitMqConnectionString = rabbitMqConnectionString ?? throw new ArgumentNullException(nameof(rabbitMqConnectionString));
            _logger = logger;
        }

        public Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                _logger?.LogInformation($"{nameof(RabbitMQLiveness)} is checking the RabbitMQ host.");

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(_rabbitMqConnectionString)
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    _logger?.LogInformation($"The {nameof(RabbitMQLiveness)} check success.");

                    return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(RabbitMQLiveness)} check fail with the exception {ex.ToString()}.");

                var message = !context.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return Task.FromResult((message, false));
            }
        }
    }
}
