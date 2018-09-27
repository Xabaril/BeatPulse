﻿using BeatPulse.Network.Core;
using System;

namespace BeatPulse.Network
{
    public class ImapLivenessOptions : ImapConnectionOptions
    {
        internal (bool Login, (string, string) Account) AccountOptions { get; private set; }
        internal (bool CheckFolder, string FolderName) FolderOptions { get; private set; }

        public void LoginWith(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            AccountOptions = (Login: true, Account: (userName, password));
        }

        public void CheckFolderExists(string inboxName)
        {
            FolderOptions = (true, inboxName);
        }
    }
}
