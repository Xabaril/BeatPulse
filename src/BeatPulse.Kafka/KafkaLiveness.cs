using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Kafka
{
    public class KafkaLiveness : IHealthCheck
    {
        private readonly Dictionary<string, object> _config;

        public KafkaLiveness(Dictionary<string, object> config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var producer = new Producer<Null, string>(_config, null, new StringSerializer(Encoding.UTF8)))
            {
                var result = await producer.ProduceAsync("beatpulse-topic", null, $"Check Kafka healthy on {DateTime.UtcNow}");

                if (result.Error.Code != ErrorCode.NoError)
                {
                    var message = $"ErrorCode {result.Error.Code} with reason ('{result.Error.Reason}')";

                    return HealthCheckResult.Failed(description: message);
                }

                return HealthCheckResult.Passed();
            }
        }
    }
}
