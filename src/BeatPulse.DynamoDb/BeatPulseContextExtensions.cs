using System;
using BeatPulse.DynamoDb;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddDynamoDb(
            this IHealthChecksBuilder builder, 
            Action<DynamoDBOptions> setupOptions, 
            string name = nameof(DynamoDbLiveness), 
            string path = "dynamodb")
        {
            var options = new DynamoDBOptions();
            setupOptions(options);

            return builder.AddCheck(name, failureStatus: null, tags: new[] { path, }, new DynamoDbLiveness(options));
        }
    }
}
