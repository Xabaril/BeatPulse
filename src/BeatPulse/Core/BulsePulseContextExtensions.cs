using System;

namespace BeatPulse.Core
{
    public static class BulsePulseContextExtensions
    {
        public static BeatPulseContext AddLiveness(this BeatPulseContext ctx, IBeatPulseLiveness liveness)
            => ctx.AddLiveness(new BeatPulseInstanceRegistration(liveness));

        public static BeatPulseContext AddLiveness(this BeatPulseContext ctx, string path, Func<IServiceProvider, IBeatPulseLiveness> creator)
            => ctx.AddLiveness(new BeatPulseFactoryRegistration(path, creator));
    }
}
