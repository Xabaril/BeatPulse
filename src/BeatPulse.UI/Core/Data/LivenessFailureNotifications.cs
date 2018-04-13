using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.UI.Core.Data
{
    public class LivenessFailureNotification
    {
        public int Id { get; set; }

        public string LivenessName { get; set; }

        public DateTime LastNotified { get; set; }
    }
}
