using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse
{
    public class BeatPulseOptions
    {
        public bool EnableOutput { get; set; }

        public string BeatPulsePath { get; set; }

        public BeatPulseOptions()
        {
            EnableOutput = true;
            BeatPulsePath = BeatPulseKeys.BEATPULSE_DEFAULT_PATH;
        }
    }
}
