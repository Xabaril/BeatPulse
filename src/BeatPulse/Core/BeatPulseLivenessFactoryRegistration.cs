﻿using System;

namespace BeatPulse.Core
{
    public class BeatPulseLivenessFactoryRegistration
        : IBeatPulseLivenessRegistration
    {
        private readonly Func<IServiceProvider, IBeatPulseLiveness> _creator;

        public string Path { get; }

        public string Name { get; }

        public BeatPulseLivenessFactoryRegistration(Func<IServiceProvider, IBeatPulseLiveness> creator, string name, string path = null)
        {
            _creator = creator ?? throw new ArgumentNullException(nameof(creator));

            Name = name;
            Path = path ?? name;
        }

        public IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider serviceProvider) => _creator.Invoke(serviceProvider);
    }
}
