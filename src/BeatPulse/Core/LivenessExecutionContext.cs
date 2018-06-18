namespace BeatPulse.Core
{
    public class LivenessExecutionContext
    {
        public bool IsDevelopment { get; private set; }

        public string Name { get; private set; }

        public string Path { get; private set; }

      
        public static LivenessExecutionContext FromRegistration(IBeatPulseLivenessRegistration registration,bool isDevelopment)
        {
            return new LivenessExecutionContext()
            {
                IsDevelopment = isDevelopment,
                Name = registration.Name,
                Path = registration.Path
            };
        }
    }
}
