using BeatPulse.Core;
using BeatPulse.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddMongoDb(this BeatPulseContext context, string mongodbConnectionString, string name = nameof(MongoDbLiveness),string defaultPath = "mongodb")
        {
            context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp=>new MongoDbLiveness(mongodbConnectionString,sp.GetService<ILogger<MongoDbLiveness>>()));
            });

            return context;
        }
    }
}
