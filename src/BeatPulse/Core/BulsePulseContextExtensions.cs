using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public static class BulsePulseContextExtensions
    {
        public static BeatPulseContext Add(this BeatPulseContext ctx, IBeatPulseLiveness liveness) 
            => ctx.Add(new BeatPulseInstanceRegistration(liveness));
        public static BeatPulseContext Add(this BeatPulseContext ctx, string path, Func<IServiceProvider, IBeatPulseLiveness> creator)
            => ctx.Add(new BeatPulseFactoryRegistration(path, creator));
    }
}
