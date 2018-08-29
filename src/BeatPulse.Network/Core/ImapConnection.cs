using System;
using System.Buffers;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BeatPulse.Network.Core
{
    public class ImapConnection : IDisposable
    {       
        public bool IsAuthenticated { get; private set; }
        public bool Connected => _tcpClient.Connected;     

        private TcpClient _tcpClient = null;
        private Stream _stream = null;        
        private readonly ImapConnectionOptions _options;
        private bool _disposed = false;

        Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> _validateRemoteCertificate = (o, c, ch, e) => true;
        

        internal ImapConnection(ImapConnectionOptions options)
        {            
            _options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(_options.Host)) throw new ArgumentNullException(nameof(_options.Host));
            if (_options.Port == default) throw new ArgumentNullException(nameof(_options.Port));
        }

        internal async Task<bool> ConnectAsync()
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_options.Host, _options.Port);

            _stream = GetStream();
            await ExecuteImapCommand(string.Empty);

            return Connected;
        }

        public async Task<bool> AuthenticateAsync(string user, string password)
        {
            var result = await ExecuteImapCommand(ImapCommands.Login(user,password));
            IsAuthenticated = result.Contains(ImapResponse.OK);
            return IsAuthenticated;
        }

        public async Task<bool> SelectFolder(string folder)
        {
            var result = await ExecuteImapCommand(ImapCommands.SelectFolder(folder));
            return result.Contains(ImapResponse.OK);
        }

        public async Task<string> GetFolders()
        {
            return await ExecuteImapCommand(ImapCommands.ListFolders());
        }

        private Stream GetStream()
        {
            var stream = _tcpClient.GetStream();

            if (_options.UseSsl)
            {
                var sslStream = GetSSLStream(stream);
                sslStream.AuthenticateAsClient(_options.Host);
                return sslStream;
            }
            else
            {
                return stream;
            }
        }

        private SslStream GetSSLStream(Stream stream)
        {

            if (_options.AllowInvalidRemoteCertificates)
            {
                return new SslStream(stream, true, new RemoteCertificateValidationCallback(_validateRemoteCertificate));
            }
            else
            {
                return new SslStream(stream);
            }
        }

        private async Task<string> ExecuteImapCommand(string command)
        {
            var buffer = Encoding.ASCII.GetBytes(command);
            await _stream.WriteAsync(buffer, 0, buffer.Length);

            var readBuffer = ArrayPool<byte>.Shared.Rent(512);
            int read = await _stream.ReadAsync(readBuffer, 0, readBuffer.Length);
            var output = Encoding.UTF8.GetString(readBuffer);

            ArrayPool<byte>.Shared.Return(readBuffer);

            return output;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _stream?.Dispose();
                _tcpClient?.Dispose();
            }
            _disposed = true;
        }
    }
}
