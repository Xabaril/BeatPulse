using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Core
{
    static class BeatPulseLivenessRegistrationCreator { 
    
        public static IBeatPulseLivenessRegistration CreateUsingOptions(BeatPulseLivenessRegistrationOptions options)
        {
            bool hasLiveness = options.Liveness != null;
            bool hasFactory = options.Factory != null;
            string path = options.Path;

            if (hasLiveness)
            {
                return new BeatPulseLivenessInstanceRegistration(options.Liveness, options.Name, path);
            }
            else
            {
                return new BeatPulseLivenessFactoryRegistration(options.Factory, options.Name, path);
            }
        }
    }
}
