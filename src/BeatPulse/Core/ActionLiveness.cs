using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.Core
{
    public class ActionLiveness
        : IBeatPulseLiveness
    {
        private readonly Func<HttpContext, CancellationToken,Task<(string, bool)>> _check;

        public ActionLiveness(Func<HttpContext,CancellationToken,Task<(string, bool)>> check)
        {
            _check = check ?? throw new ArgumentNullException(nameof(check));
        }

        public Task<(string, bool)> IsHealthy(HttpContext context, LivenessContext livenessContext,CancellationToken cancellationToken = default)
        {
            return _check(context,cancellationToken);
        }
    }
}
