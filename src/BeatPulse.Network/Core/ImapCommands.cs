using System;
using System.Collections.Generic;
using System.Text;

namespace BeatPulse.Network.Core
{
    public class ImapCommands
    {
        public static string Login(string user, string password) => $"& login {user} {password} \r\n";
        public static string SelectFolder(string folder) => $"& SELECT {folder}\r\n";
        public static string ListFolders() => "& LIST " + "\"\"" + " \"*\"" + "\r\n";
    }

    public class ImapResponse
    {
        public static string OK = "& OK";
        public static string ERROR = "& NO";
    }
}
