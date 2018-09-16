using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BeatPulse.System
{
    public class MaximumValueLiveness<T> : IHealthCheck
        where T : IComparable<T>
    {
        private readonly T maximunValue;
        private readonly Func<T> currentValueFunc;

        public MaximumValueLiveness(T maximunValue, Func<T> currentValueFunc)
        {
            this.maximunValue = maximunValue;
            this.currentValueFunc = currentValueFunc;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var currentValue = currentValueFunc();

            if (currentValue.CompareTo(maximunValue) <= 0)
            {
                return Task.FromResult(HealthCheckResult.Passed());
            }

            return Task.FromResult(HealthCheckResult.Failed($"Maximun={maximunValue}, Current={currentValue}"));
        }
    }
}
