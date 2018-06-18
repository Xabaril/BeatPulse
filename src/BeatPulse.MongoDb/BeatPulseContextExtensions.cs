using BeatPulse.Core;
using BeatPulse.MongoDb;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMongoDb(this BeatPulseContext context, string mongodbConnectionString, string name = nameof(MongoDbLiveness),string defaultPath = "mongodb")
        {
            context.AddLiveness(name, setup =>
            {
                setup.UseLiveness(new MongoDbLiveness(mongodbConnectionString));
                setup.UsePath(defaultPath);
            });

            return context;
        }
    }
}
