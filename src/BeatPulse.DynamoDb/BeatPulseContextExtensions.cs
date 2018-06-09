using BeatPulse.Core;
using BeatPulse.DynamoDb;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddDynamoDb(this BeatPulseContext context, Action<DynamoDBOptions> setupOptions, string name = nameof(DynamoDbLiveness), string defaultPath = "dynamodb")
        {
            var options = new DynamoDBOptions();
            setupOptions(options);

            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseLiveness(new DynamoDbLiveness(options));
            });
        }
    }
}
