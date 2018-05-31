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

        public async Task<(string, bool)> IsHealthy(HttpContext context, LivenessContext livenessContext, CancellationToken cancellationToken = default)
        {
            var isDevelopment = livenessContext.IsDevelopment;
            var name = livenessContext.Name;
            try
            {
                using (var producer = new Producer<Null, string>(_config, null, new StringSerializer(Encoding.UTF8)))
                {
                    var result = await producer.ProduceAsync("beatpulse-topic",null, $"Check Kafka healthy on {DateTime.UtcNow}");

                    if (result.Error.Code != ErrorCode.NoError)
                    {
                        var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, name)
                            : $"ErrorCode {result.Error.Code} with reason ('{result.Error.Reason}')";
                        return (message, false);
                    }
                    return (BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true);
                }
            }
            catch (Exception ex)
            {
                var message = !isDevelopment ? string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, name)
                    : $"Exception {ex.GetType().Name} with message ('{ex.Message}')";

                return (message, false);
            }
        }
    }
}
