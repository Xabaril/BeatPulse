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

        public KafkaLiveness(Dictionary<string, object> config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessExecutionContext livenessContext, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var producer = new Producer<Null, string>(_config, null, new StringSerializer(Encoding.UTF8)))
                {
                    var result = await producer.ProduceAsync("beatpulse-topic",null, $"Check Kafka healthy on {DateTime.UtcNow}");

                    if (result.Error.Code != ErrorCode.NoError)
                    {
                        var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                            : $"ErrorCode {result.Error.Code} with reason ('{result.Error.Reason}')";

                        return (message, false);
                    }

                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                var message = !livenessContext.IsDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, livenessContext.Name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
