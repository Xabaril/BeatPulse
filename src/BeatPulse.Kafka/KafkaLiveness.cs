using BeatPulse.Core;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Kafka
{
    public class KafkaLiveness : IBeatPulseLiveness
    {
        private readonly Dictionary<string, object> _configuration;
        private readonly ILogger<KafkaLiveness> _logger;

        public KafkaLiveness(Dictionary<string, object> configuration,ILogger<KafkaLiveness> logger = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(KafkaLiveness)} is checking the Kafka broker.");

                using (var producer = new Producer<Null, string>(_configuration, null, new StringSerializer(Encoding.UTF8)))
                {
                    var result = await producer.ProduceAsync("beatpulse-topic",null, $"Check Kafka healthy on {DateTime.UtcNow}");

                    if (result.Error.Code != ErrorCode.NoError)
                    {
                        _logger?.LogWarning($"The {nameof(KafkaLiveness)} check failed.");

                        return LivenessResult.UnHealthy($"ErrorCode {result.Error.Code} with reason ('{result.Error.Reason}')");
                    }

                    _logger?.LogInformation($"The {nameof(KafkaLiveness)} check success.");

                    return LivenessResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning($"The {nameof(KafkaLiveness)} check fail for Kafka broker with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }
    }
}
