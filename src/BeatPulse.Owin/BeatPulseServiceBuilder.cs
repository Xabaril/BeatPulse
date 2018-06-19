using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin
{
    public class BeatPulseServiceBuilder
    {

        public IBeatPulseService Buid(Action<BeatPulseContext> setupAction = null)
        {
            var ctx = new BeatPulseContext();
            setupAction?.Invoke(ctx);
            return new OwinBeatPulseService(ctx);
        }
    }
}
