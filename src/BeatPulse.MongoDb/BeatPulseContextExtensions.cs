using BeatPulse.Core;
using BeatPulse.MongoDb;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMongoDb(this BeatPulseContext context, string mongodbConnectionString, string defaultPath = "mongodb")
        {
            context.AddLiveness(new MongoDbLiveness(mongodbConnectionString, defaultPath));

            return context;
        }
    }
}
