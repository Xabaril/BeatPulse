using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public class BeatPulseFactoryRegistration : IBeatPulseLivenessRegistration
    {
        private readonly Func<IServiceProvider, IBeatPulseLiveness> _creator;

        public string Path { get; }
        public Guid Id { get; }

        public BeatPulseFactoryRegistration(string path, Func<IServiceProvider, IBeatPulseLiveness> creator)
        {
            _creator = creator ?? throw new ArgumentNullException(nameof(creator));
            Id = Guid.NewGuid();
            Path = path;
        }

        public IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider sp) => _creator.Invoke(sp);
    }
}
