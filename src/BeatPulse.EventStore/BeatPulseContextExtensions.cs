using BeatPulse.Core;
using BeatPulse.EventStore;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddEventStore(this BeatPulseContext context, string connectionString, string login = null, string password = null, string name = nameof(EventStoreLiveness), string defaultPath = "eventstore")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp => new EventStoreLiveness(connectionString, login, password));
            });
        }
    }
}
