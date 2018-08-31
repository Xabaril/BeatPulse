using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Hosted
{
    public interface IHostedBeatPulseEndpoint
    {
        Task OpenAsync();
        void Stop();

        Func<string, Task<IEnumerable<LivenessResult>>>  OnRequestReceivedAsync { get; set; }

        void Setup(BeatPulseHostedOptions options);

    }
}
