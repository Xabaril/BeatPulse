using BeatPulse.Core;
using BeatPulse.IdSvr;
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
                setup.UseLiveness(new IdSvrLiveness(idSvrUri));
            });
        }
    }
}
