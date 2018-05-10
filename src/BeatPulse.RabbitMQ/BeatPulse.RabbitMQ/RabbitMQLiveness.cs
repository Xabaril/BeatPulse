using Rmq = RabbitMQ;

using System;
using System.Threading;
using System.Threading.Tasks;

using BeatPulse.Core;

using Microsoft.AspNetCore.Http;

namespace BeatPulse.RabbitMQ
{
    public class RabbitMQLiveness : IBeatPulseLiveness
    {
        private readonly string _rabbitMqConnectionString;

        public RabbitMQLiveness(string rabbitMqConnectionString)
        {
            _rabbitMqConnectionString = rabbitMqConnectionString ?? throw new ArgumentNullException(nameof(rabbitMqConnectionString));
        }

        public string Name => nameof(RabbitMQLiveness);

        public string DefaultPath => "rabbitmq";

        public Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var factory = new Rmq.Client.ConnectionFactory();
                factory.Uri = new Uri(_rabbitMqConnectionString);

                using (var connection = factory.CreateConnection())
                {
                    if (connection.IsOpen)
                    {
                        return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
                    }
                }

                return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, false));
            }
            catch (Exception ex)
            {
                var message = !isDevelopment
                    ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return Task.FromResult((message, false));
            }
        }
    }
}
