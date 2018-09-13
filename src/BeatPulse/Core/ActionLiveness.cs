using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public class ActionLiveness
        : IBeatPulseLiveness
    {
        private readonly Func<CancellationToken,Task<LivenessResult>> _check;

        public ActionLiveness(Func<CancellationToken,Task<LivenessResult>> check)
        {
            _check = check ?? throw new ArgumentNullException(nameof(check));
        }

        public Task<LivenessResult> IsHealthy(LivenessExecutionContext context,CancellationToken cancellationToken = default)
        {
            return _check(cancellationToken);
        }
    }
}
