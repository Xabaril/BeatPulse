using BeatPulse.Core;
using BeatPulse.RabbitMQ;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddRabbitMQ(this BeatPulseContext context, string rabbitMQConnectionString, string name = nameof(RabbitMQLiveness), string defaultPath = "rabbitmq")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new RabbitMQLiveness(rabbitMQConnectionString));
            });
        }
    }
}
