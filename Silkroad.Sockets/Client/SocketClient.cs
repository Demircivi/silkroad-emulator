using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Silkroad.Sockets.Client.Models;

namespace Silkroad.Sockets.Client
{
    internal class SocketClient
    {
        #region Events

        public delegate void DataReceivedDelegate(SocketClient socketClient, byte[] data);
        public delegate void DisconnectedDelegate(SocketClient socketClient);
        
        public event DataReceivedDelegate DataReceived;
        public event DisconnectedDelegate Disconnected; 
        
        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }
        
        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this);
        }
        
        #endregion

        private const int BufferSize = 8 * 1024;
        
        private Socket _socket;
        private readonly byte[] _buffer;
        private bool _isDisconnected;
        public SocketClientId Id { get; private set; }

        public SocketClient(Socket socket)
        {
            _socket = socket;
            _buffer = new byte[BufferSize];
            Id = SocketClientId.New();
        }

        public void Start()
        {
            Receive();
        }

        public void Disconnect()
        {
            _isDisconnected = true;

            OnDisconnected();
        }
        
        private async void Receive()
        {
            do
            {
                try
                {
                    var bytesRead = await _socket.ReceiveAsync(_buffer, SocketFlags.None);
                    
                    var data = new byte[bytesRead];

                    Buffer.BlockCopy(_buffer, 0, data, 0, bytesRead);

                    OnDataReceived(data);
                }
                catch (SocketException socketException)
                {
                    Disconnect();
                }
            } while (!_isDisconnected);
        }

        public async Task Send(byte[] buffer)
        {
            var bytesSent = await _socket.SendAsync(buffer, SocketFlags.None);
        }
    }
}
