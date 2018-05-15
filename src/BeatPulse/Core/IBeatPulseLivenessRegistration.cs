using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    public interface IBeatPulseLivenessRegistration
    {
        Guid Id { get; }
        string Path { get; }
        IBeatPulseLiveness GetOrCreateLiveness(IServiceProvider sp);
    }

}
