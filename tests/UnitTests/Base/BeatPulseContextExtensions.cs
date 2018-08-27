using UnitTests.Base;

namespace BeatPulse.Core
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContextAssertions Should(this BeatPulseContext context)
        {
            return new BeatPulseContextAssertions(context);
        }
    }
}
