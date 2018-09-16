using System.Collections.Generic;
using BeatPulse.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddKafka(this IHealthChecksBuilder builder, Dictionary<string, object> config, string name = nameof(KafkaLiveness), string defaultPath = "kafka")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new KafkaLiveness(config));
        }
    }
}
