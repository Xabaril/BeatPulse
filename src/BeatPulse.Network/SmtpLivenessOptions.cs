using BeatPulse.Network.Core;
using System;

namespace BeatPulse.Network
{
    public class SmtpLivenessOptions : SmtpConnectionOptions {
        
        internal (bool login, (string, string) account) AccountOptions { get; private set; }
        public void LoginWith(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            AccountOptions = (login: true, account: (userName, password));
        }      
    }  
}
