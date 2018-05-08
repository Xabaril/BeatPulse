using BeatPulse.Core;
using BeatPulse.IdSvr;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddIdentityServer(this BeatPulseContext context, Uri idSvrUri)
        {
            context.Add(new IdSvrLiveness(idSvrUri));
            return context;
        }
    }
}
