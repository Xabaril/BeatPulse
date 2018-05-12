using BeatPulse.Core;
using BeatPulse.RabbitMQ;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRabbitMQ(this BeatPulseContext context, string rabbitMQConnectionString, string defaultPath = "rabbitmq")
        {
            context.Add(new RabbitMQLiveness(rabbitMQConnectionString, defaultPath));

            return context;
        }
    }
}
