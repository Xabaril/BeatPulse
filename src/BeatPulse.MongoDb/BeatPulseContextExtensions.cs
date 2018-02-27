using BeatPulse.Core;
using BeatPulse.MongoDb;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMongoDb(this BeatPulseContext context, string mongodbConnectionString)
        {
            context.Add(new MongoDbLiveness(mongodbConnectionString));

            return context;
        }
    }
}
