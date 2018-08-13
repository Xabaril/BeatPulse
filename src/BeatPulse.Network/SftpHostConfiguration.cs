using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeatPulse.Network
{
    public class SftpHostConfiguration
    {
        internal string Host { get; }
        internal int Port { get; }
        internal string UserName { get; set; }
        
        internal List<AuthenticationMethod> AuthenticationMethods { get; } = new List<AuthenticationMethod>();

        public SftpHostConfiguration(string host, int port)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            if (host == default) throw new ArgumentNullException(nameof(port));
        }

        public void UsePasswordAuthentication(string userName, string password)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            AuthenticationMethods.Clear();

            AuthenticationMethods.Add(new PasswordAuthenticationMethod(userName, password));
        }

        public void UsePrivateKeyAuthentication(string userName, string privateKey, string passphrase)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName)); ;
            if (string.IsNullOrEmpty(privateKey)) throw new ArgumentNullException(nameof(privateKey));
            if (string.IsNullOrEmpty(passphrase)) throw new ArgumentNullException(nameof(passphrase));

            AuthenticationMethods.Clear();

            var keyBytes = Encoding.UTF8.GetBytes(privateKey);

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(keyBytes, 0, keyBytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var privateKeyFile = new PrivateKeyFile(memoryStream, passphrase);

                AuthenticationMethods.Add(new PrivateKeyAuthenticationMethod(UserName, privateKeyFile));
            }
        }
    }
}
