using System;
using BeatPulse.IdSvr;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulse
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddIdentityServer(this IHealthChecksBuilder builder, Uri idSvrUri, string name = nameof(IdSvrLiveness),string defaultPath = "idsvr")
        {
            return builder.AddCheck(name, failureStatus: null, tags: new[] { defaultPath, }, new IdSvrLiveness(idSvrUri));
        }
    }
}
