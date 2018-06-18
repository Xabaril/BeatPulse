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

        Func<Task<bool>>  OnRequestReceivedAsync { get; set; }

    }
}
