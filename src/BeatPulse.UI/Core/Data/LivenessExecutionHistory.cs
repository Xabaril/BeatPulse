using System;

namespace BeatPulse.UI.Core.Data
{

    internal class LivenessExecutionHistory
    {
        public int Id { get; set; }

        public DateTime ExecutedOn { get; set; }

        public string LivenessUri { get; set; }

        public string LivenessName { get; set; }

        public bool IsHealthy { get; set; }

        public string Result { get; set; }
    }
}
