namespace BeatPulse.Core
{
    public class LivenessExecutionContext
    {
        public string Name { get; private set; }

        public string Path { get; private set; }


        internal static LivenessExecutionContext FromRegistration(IBeatPulseLivenessRegistration registration, bool showDetailedErrors)
        {
            return new LivenessExecutionContext()
            {
                Name = registration.Name,
                Path = registration.Path
            };
        }
    }
}
