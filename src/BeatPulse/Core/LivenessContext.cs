using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public class LivenessContext
    {
        public bool IsDevelopment { get; internal set; }
        public string Name { get; internal set; }
        public string Path { get; internal set; }
        public int Index { get; internal set; }

    }
}
