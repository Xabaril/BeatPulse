using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
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

        public Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
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

                    return Task.FromResult(
                        LivenessResult.Healthy());
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(RabbitMQLiveness)} check fail with the exception {ex.ToString()}.");

                return Task.FromResult(
                        LivenessResult.UnHealthy(ex));
            }
        }
    }
}
