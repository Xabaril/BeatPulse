namespace BeatPulse.Core
{
    public class LivenessExecutionContext
    {
        //public bool ShowDetailedErrors { get; private set; }

        public string Name { get; private set; }

        public string Path { get; private set; }


        internal static LivenessExecutionContext FromRegistration(IBeatPulseLivenessRegistration registration, bool showDetailedErrors)
        {
            return new LivenessExecutionContext()
            {
                //ShowDetailedErrors = showDetailedErrors,
                Name = registration.Name,
                Path = registration.Path
            };
        }
    }
}
