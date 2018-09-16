using BeatPulse.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddRabbitMQ(this IHealthChecksBuilder builder, string rabbitMQConnectionString, string name = nameof(RabbitMQLiveness), string defaultPath = "rabbitmq")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new RabbitMQLiveness(rabbitMQConnectionString));
        }
    }
}
