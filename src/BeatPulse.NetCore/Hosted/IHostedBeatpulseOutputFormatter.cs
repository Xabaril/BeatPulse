using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Hosted
{

    public interface IHostedBeatpulseOutputFormatter
    {
        byte[] Serialize(OutputLivenessMessage output, BeatPulseOptions options);
    }

}
