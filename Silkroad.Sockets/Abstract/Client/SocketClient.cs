using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Silkroad.Sockets.Abstract.Client.Enums;
using Silkroad.Sockets.Abstract.Client.Exceptions;
using Silkroad.Sockets.Abstract.Client.Models;

namespace Silkroad.Sockets.Abstract.Client
{
    internal class SocketClient
    {
        #region Events

        public delegate void DataReceivedDelegate(SocketClient socketClient, byte[] data);
        public delegate void DisconnectedDelegate(SocketClient socketClient, SocketClientDisconnectType disconnectType);
        
        public event DataReceivedDelegate DataReceived;
        public event DisconnectedDelegate Disconnected; 
        
        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }
        
        protected virtual void OnDisconnected(SocketClientDisconnectType disconnectType)
        {
            Disconnected?.Invoke(this, disconnectType);
        }
        
        #endregion

        private const int BufferSize = 8 * 1024;
        private const int PingMaximumMilliseconds = 30 * 1000;
        
        private readonly Socket _socket;
        private readonly byte[] _buffer;
        private bool _isDisconnected;
        public SocketClientId Id { get; }

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

            _socket.Disconnect(false);
            
            // NOTE: We're not calling OnDisconnect() because Receive() will call it when it receives SocketException
        }

        private async void Receive()
        {
            do
            {
                try
                {
                    var receiveTask = _socket.ReceiveAsync(_buffer, SocketFlags.None);
                    var bytesRead = await Task.WhenAny(Task.Delay(PingMaximumMilliseconds), receiveTask);

                    if (bytesRead != receiveTask)
                    {
                        throw new SocketClientTimeoutException();
                    }

                    var data = new byte[receiveTask.Result];

                    Buffer.BlockCopy(_buffer, 0, data, 0, receiveTask.Result);

                    OnDataReceived(data);
                }
                catch (SocketClientTimeoutException ex)
                {
                    Disconnect();

                    OnDisconnected(SocketClientDisconnectType.HighPing);
                }
                catch (Exception ex) when (
                    ex is SocketException ||
                    ex is AggregateException && ex.InnerException is SocketException
                )
                {
                    Disconnect();

                    OnDisconnected(SocketClientDisconnectType.Disconnect);
                }
            } while (!_isDisconnected);
        }

        public async Task Send(byte[] buffer)
        {
            try
            {
                var bytesSent = await _socket.SendAsync(buffer, SocketFlags.None);

                Console.WriteLine($"Sent {bytesSent} bytes");
            }
            catch
            {
                // Ignored because the socket is disconnected when we get here, receive will take care of disconnection
            }
        }
    }
}
