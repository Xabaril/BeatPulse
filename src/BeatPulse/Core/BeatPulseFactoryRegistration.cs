using System;

namespace BeatPulse.Core
{
    public class BeatPulseFactoryRegistration : IBeatPulseLivenessRegistration
    {
        private readonly Func<IServiceProvider, IBeatPulseLiveness> _creator;

        public string Path { get; }
       
        public BeatPulseFactoryRegistration(string path, Func<IServiceProvider, IBeatPulseLiveness> creator)
        {
            Path = path;
            _creator = creator ?? throw new ArgumentNullException(nameof(creator));
        }

        public IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider sp) => _creator.Invoke(sp);
    }
}
