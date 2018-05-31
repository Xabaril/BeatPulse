using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public class BeatPulseLivenessRegistrationOptions
    {
        internal string Path { get; private set; }
        internal IBeatPulseLiveness Liveness { get; private set; }
        internal Func<IServiceProvider, IBeatPulseLiveness> Factory { get; private set; }
        internal string Name { get; }

        public BeatPulseLivenessRegistrationOptions(string name)
        {
            Name = name ?? throw new ArgumentNullException($"{nameof(BeatPulseLivenessRegistrationOptions)}.{nameof(name)}");
            Path = name;
        }
        
        public void UsePath(string path) 
            => Path = path ?? throw new ArgumentNullException($"{nameof(BeatPulseLivenessRegistrationOptions)}.{nameof(path)}");
        public void UseLiveness(IBeatPulseLiveness liveness) 
            => Liveness = liveness ?? throw new ArgumentNullException($"{nameof(BeatPulseLivenessRegistrationOptions)}.{nameof(liveness)}");
        public void UseFactory(Func<IServiceProvider, IBeatPulseLiveness> factory) 
            => Factory = factory ?? throw new ArgumentNullException($"{nameof(BeatPulseLivenessRegistrationOptions)}.{nameof(factory)}");
    }
}
