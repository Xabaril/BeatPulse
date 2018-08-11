using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.System.Extensions
{
    public static class LiveExecutionContextExtensions
    {
        public static (string, bool) CreateErrorResponse(this LivenessExecutionContext context, string message)
        {
            return (context.IsDevelopment ? message : string.Format(BeatPulseKeys.BEATPULSE_HEALTHCHECK_DEFAULT_ERROR_MESSAGE, context.Name), false);
        }
    }
}
