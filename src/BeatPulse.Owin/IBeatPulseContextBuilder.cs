using BeatPulse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Owin
{
    public interface IBeatPulseContextBuilder
    {
        void With(Action<BeatPulseContext> contextSetup);
    }
}
