using System;

namespace BeatPulse.UI.Core.Data
{

    internal class LivenessExecutionHistory
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public bool IsHealthy { get; set; }

        public DateTime OnStateFrom { get; set; }

        public DateTime LastExecuted { get; set; }

        public string LivenessUri { get; set; }

        public string LivenessName { get; set; }

        public string LivenessResult { get; set; }
    }
}
