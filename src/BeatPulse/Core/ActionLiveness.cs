using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.Core
{
    public class ActionLiveness : IHealthCheck
    {
        private readonly Func<CancellationToken, Task<HealthCheckResult>> _check;

        public ActionLiveness(Func<CancellationToken, Task<HealthCheckResult>> check)
        {
            _check = check ?? throw new ArgumentNullException(nameof(check));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return _check(cancellationToken);
        }
    }
}
