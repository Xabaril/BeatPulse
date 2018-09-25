using System;

namespace BeatPulse.Core
{
    public class BeatPulseLivenessRegistrationOptions
    {
        internal string Path { get; private set; }

        internal string Name { get; private set; }

        internal IBeatPulseLiveness Liveness { get; private set; }

        internal Func<IServiceProvider, IBeatPulseLiveness> Factory { get; private set; }

        public BeatPulseLivenessRegistrationOptions(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = name; //out-of-box the path is selected name
        }

        public void UsePath(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public void UseLiveness(IBeatPulseLiveness liveness)
        {
            Liveness = liveness ?? throw new ArgumentNullException(nameof(liveness));
        }

        public void UseFactory(Func<IServiceProvider, IBeatPulseLiveness> factory)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        internal IBeatPulseLivenessRegistration CreateRegistration()
        {
            if (Liveness != null)
            {
                return new BeatPulseLivenessInstanceRegistration(Liveness, Name, Path);
            }
            else if (Factory != null)
            {
                return new BeatPulseLivenessFactoryRegistration(Factory, Name, Path);
            }
            else
            {
                throw new InvalidOperationException($"The livenes {Name} is not configured with existing liveness or factory");
            }
        }
    }
}
