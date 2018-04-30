using System;
using System.Collections.Generic;

namespace BeatPulse.UI.Core.Data
{

    internal class LivenessExecution
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public bool IsHealthy { get; set; }

        public DateTime OnStateFrom { get; set; }

        public DateTime LastExecuted { get; set; }

        public string LivenessUri { get; set; }

        public string LivenessName { get; set; }

        public string LivenessResult { get; set; }

        public List<LivenessExecutionHistory> History { get; set; }
    }
}
