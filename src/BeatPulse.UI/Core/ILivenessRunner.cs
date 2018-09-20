using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.UI.Core
{
    interface ILivenessRunner
    {
        Task Run(CancellationToken cancellationToken);
    }
}
