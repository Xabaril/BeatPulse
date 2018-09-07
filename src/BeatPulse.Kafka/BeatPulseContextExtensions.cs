using BeatPulse.Core;
using BeatPulse.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddKafka(this BeatPulseContext context, Dictionary<string, object> config, string name = nameof(KafkaLiveness), string defaultPath = "kafka")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp => new KafkaLiveness(config, sp.GetService<ILogger<KafkaLiveness>>()));
            });
        }
    }
}
