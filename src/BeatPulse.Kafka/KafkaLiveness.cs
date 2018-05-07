using BeatPulse.Core;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Kafka
{
    public class KafkaLiveness : IBeatPulseLiveness
    {
        private readonly Dictionary<string, object> _config;

        public string Name => nameof(KafkaLiveness);

        public string DefaultPath => "kafka";

        public KafkaLiveness(Dictionary<string, object> config)
        {
            _config = config;
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, bool isDevelopment, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var producer = new Producer<Null, string>(_config, null, new StringSerializer(Encoding.UTF8)))
                {
                    var result = await producer.ProduceAsync("betapulse-topic", null, $"Check Kafka healthy on {DateTime.UtcNow}");

                    if (result.Error.Code != ErrorCode.NoError)
                    {
                        var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                            : $"ErrorCode {result.Error.Code} with reason ('{result.Error.Reason}')";
                        return (message, false);
                    }
                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
