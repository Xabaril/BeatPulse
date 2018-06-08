using BeatPulse.AWS.DynamoDB;
using BeatPulse.Core;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddAWSDynamoDB(this BeatPulseContext context, Action<AWSDynamoDBOptions> setupOptions, string defaultPath = "awsdynamodb")
        {
            var options = new AWSDynamoDBOptions();
            setupOptions(options);

            context.AddLiveness(new AWSDynamoDBLiveness(options, defaultPath));
            return context;
        }
    }
}
