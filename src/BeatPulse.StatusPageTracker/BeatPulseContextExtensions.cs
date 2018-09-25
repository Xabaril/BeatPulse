using BeatPulse.Core;
using BeatPulse.StatusPageTracker;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddStatusPageTracker(this BeatPulseContext context,Action<StatusPageComponent> setup)
        {
            var component = new StatusPageComponent();
            setup(component);
          
            context.AddTracker(new StatusPageIOTracker(component));

            return context;
        }
    }
}
