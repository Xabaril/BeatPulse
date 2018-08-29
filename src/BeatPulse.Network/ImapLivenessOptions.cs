using BeatPulse.Network.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Network
{
    public class ImapLivenessOptions : ImapConnectionOptions
    {
        internal (bool login, (string, string) account) AccountOptions { get; private set; }
        internal (bool checkFolder, string folderName) FolderOptions { get; private set; }

        public void LoginWith(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            AccountOptions = (login: true, account: (userName, password));
        }

        public void CheckFolderExists(string inboxName)
        {
            FolderOptions = (true, inboxName);
        }
    }
}
