﻿using System;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Network.Core
{
    internal class SmtpConnection: MailConnection
    {
        private readonly SmtpConnectionOptions _options;
        private SmtpConnectionType _connectionType;

        public SmtpConnectionType ConnectionType
        {
            get
            {
                return _connectionType;
            }

            private set
            {
                _connectionType = value;
                UseSSL = ConnectionType == SmtpConnectionType.SSL ? true : false;
            }
        }

        public SmtpConnection(SmtpConnectionOptions options) 
            : base(options.Host, options.Port, false, options.AllowInvalidRemoteCertificates)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            ConnectionType = _options.ConnectionType;
            ComputeDefaultValues();
        }

        private void ComputeDefaultValues()
        {
            switch (_options.ConnectionType)
            {
                case SmtpConnectionType.AUTO when Port == 465:
                    ConnectionType = SmtpConnectionType.SSL;
                    break;
                case SmtpConnectionType.AUTO when Port == 587:
                    ConnectionType = SmtpConnectionType.TLS;
                    break;
                    
            }

            if (ConnectionType == SmtpConnectionType.AUTO)
            {
                throw new Exception($"Port {Port} is not a valid smtp port when using automatic configuration");
            }
        }

        public new async Task<bool> ConnectAsync()
        {
            await base.ConnectAsync();
            var result = await ExecuteCommand(SmtpCommands.EHLO(Host));
            return result.Contains(SmtpResponse.ACTION_OK);
        }

        public async Task<bool> AuthenticateAsync(string userName, string password)
        {
            if(!UseSSL)
            {
                await UpgradeToSecureConnection();
            }
            await ExecuteCommand(SmtpCommands.EHLO(Host));
            await ExecuteCommand(SmtpCommands.AUTHLOGIN());
            await ExecuteCommand($"{ ToBase64(userName)}\r\n");
            var result = await ExecuteCommand($"{ToBase64(password)}\r\n");
            return result.Contains(SmtpResponse.AUTHENTICATION_SUCCESS);
        }

        private async Task<bool> UpgradeToSecureConnection()
        {
            var upgradeResult = await ExecuteCommand(SmtpCommands.STARTTLS());
            if (upgradeResult.Contains(SmtpResponse.SERVICE_READY))
            {
                UseSSL = true;
                _stream = GetStream();
                return true;
            }
            else
            {
                throw new Exception("Could not upgrade SMTP non SSL connection using STARTTLS handshake");
            }
        }

        private string ToBase64(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }
    }
}
