using BeatPulse.Core;
using BeatPulse.IdSvr;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddIdentityServer(this BeatPulseContext context, Uri idSvrUri, string name = nameof(IdSvrLiveness),string defaultPath = "idsvr")
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp=>new IdSvrLiveness(idSvrUri,sp.GetService<ILogger<IdSvrLiveness>>()));
            });
        }
    }
}
