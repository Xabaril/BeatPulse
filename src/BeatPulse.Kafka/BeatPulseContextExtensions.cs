using BeatPulse.Core;
using BeatPulse.Kafka;
using System.Collections.Generic;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddKafka(this BeatPulseContext context, Dictionary<string, object> config, string defaultPath = "kafka")
        {
            context.AddLiveness(new KafkaLiveness(config, defaultPath));
            return context;
        }
    }
}
