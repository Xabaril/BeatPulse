using BeatPulse.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rmq = RabbitMQ;

namespace BeatPulse.RabbitMQ
{
    public class RabbitMQLiveness : IBeatPulseLiveness
    {
        private readonly string _rabbitMqConnectionString;

        public RabbitMQLiveness(string rabbitMqConnectionString)
        {
            _rabbitMqConnectionString = rabbitMqConnectionString ?? throw new ArgumentNullException(nameof(rabbitMqConnectionString));
        }

        public Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = new CancellationToken())
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
                        return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
                    }
                }

                return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, false));
            }
            catch (Exception ex)
            {
                var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return Task.FromResult((message, false));
            }
        }
    }
}
