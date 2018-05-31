using BeatPulse.Core;
using BeatPulse.MongoDb;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMongoDb(this BeatPulseContext context, string mongodbConnectionString, string defaultPath = "mongodb")
        {
            context.AddLiveness(nameof(MongoDbLiveness), opt =>
            {
                opt.UseLiveness(new MongoDbLiveness(mongodbConnectionString));
                opt.UsePath(defaultPath);
            });

            return context;
        }
    }
}
