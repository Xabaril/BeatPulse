using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public class ActionLiveness
        : IBeatPulseLiveness
    {
        private readonly Func<CancellationToken,Task<(string, bool)>> _check;

        public ActionLiveness(Func<CancellationToken,Task<(string, bool)>> check)
        {
            _check = check ?? throw new ArgumentNullException(nameof(check));
        }

        public Task<(string, bool)> IsHealthy(LivenessExecutionContext context,CancellationToken cancellationToken = default)
        {
            return _check(cancellationToken);
        }
    }
}
