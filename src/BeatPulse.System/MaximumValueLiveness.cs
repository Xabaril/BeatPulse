using BeatPulse.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BeatPulse.System
{
    public class MaximumValueLiveness<T> : IBeatPulseLiveness
        where T : IComparable<T>
    {
        private readonly T maximunValue;
        private readonly Func<T> currentValueFunc;

        public MaximumValueLiveness(T maximunValue, Func<T> currentValueFunc)
        {
            this.maximunValue = maximunValue;
            this.currentValueFunc = currentValueFunc;
        }

        public Task<(string, bool)> IsHealthy(LivenessExecutionContext context, CancellationToken cancellationToken = default)
        {
            var currentValue = currentValueFunc();

            if (currentValue.CompareTo(maximunValue) <= 0)
            {
                return Task.FromResult((BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_OK_MESSAGE, true));
            }

            return Task.FromResult(($"Maximun={maximunValue}, Current={currentValue}", false));
        }
    }
}
