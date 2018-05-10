using BeatPulse.Core;
using BeatPulse.RabbitMQ;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRabbitMQ(this BeatPulseContext context, string rabbitMQConnectionString)
        {
            context.Add(new RabbitMQLiveness(rabbitMQConnectionString));

            return context;
        }
    }
}
