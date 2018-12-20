using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    interface ILivenessRunner : IDisposable
    {
        Task Run(CancellationToken cancellationToken);
    }
}
