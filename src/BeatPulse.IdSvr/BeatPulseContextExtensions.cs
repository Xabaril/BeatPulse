using BeatPulse.Core;
using BeatPulse.IdSvr;
using System;

namespace BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddIdentityServer(this BeatPulseContext context, Uri idSvrUri, string defaultPath = "idsvr")
        {

            return context.AddLiveness(nameof(IdSvrLiveness), opt =>
            {
                opt.UsePath(defaultPath);
                opt.UseLiveness(new IdSvrLiveness(idSvrUri));
            });

        }
    }
}
