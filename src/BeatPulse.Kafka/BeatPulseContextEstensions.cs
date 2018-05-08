using BeatPulse.Core;
using BeatPulse.Kafka;
using System.Collections.Generic;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddKafka(this BeatPulseContext context, Dictionary<string, object> config)
        {
            context.Add(new KafkaLiveness(config));
            return context;
        }
    }
}
