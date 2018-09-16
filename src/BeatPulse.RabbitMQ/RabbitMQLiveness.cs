using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Rmq = RabbitMQ;

namespace BeatPulse.RabbitMQ
{
    public class RabbitMQLiveness : IHealthCheck
    {
        private readonly string _rabbitMqConnectionString;

        public RabbitMQLiveness(string rabbitMqConnectionString)
        {
            _rabbitMqConnectionString = rabbitMqConnectionString ?? throw new ArgumentNullException(nameof(rabbitMqConnectionString));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var factory = new Rmq.Client.ConnectionFactory()
                {
                    Uri = new Uri(_rabbitMqConnectionString)
                };

                using (var connection = factory.CreateConnection())
                {
                    if (connection.IsOpen
                        &&
                        connection.ServerProperties.Any())
                    {
                        return Task.FromResult(HealthCheckResult.Passed());
                    }
                }

                return Task.FromResult(HealthCheckResult.Failed());
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Failed(exception: ex));
            }
        }
    }
}
