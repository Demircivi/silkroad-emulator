using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Silkroad.Shared.Lists;
using Silkroad.Sockets.Abstract.Client;
using Silkroad.Sockets.Abstract.Client.Enums;
using Silkroad.Sockets.Abstract.Client.Models;
using Silkroad.Sockets.Abstract.Server.Exceptions;

namespace Silkroad.Sockets.Abstract.Server
{
    // TODO: Support for multiple start/stops sequences? Reusability
    internal sealed class SocketServer
    {
        #region Events

        public delegate void ConnectedDelegate(SocketClientId id);
        public delegate void DataReceivedDelegate(SocketClientId id, byte[] data);
        public delegate void DisconnectedDelegate(SocketClientId id, SocketClientDisconnectType disconnectType);
        
        public event ConnectedDelegate Connected;
        public event DataReceivedDelegate DataReceived;
        public event DisconnectedDelegate Disconnected;

        private void OnConnected(SocketClientId id)
        {
            Connected?.Invoke(id);
        }

        private void OnDataReceived(SocketClientId id, byte[] data)
        {
            DataReceived?.Invoke(id, data);
        }

        private void OnDisconnected(SocketClientId id, SocketClientDisconnectType disconnectType)
        {
            Disconnected?.Invoke(id, disconnectType);
        }
        
        #endregion

        private bool _isStarted;
        private Socket _listenerSocket;
        private readonly ThreadSafeList<SocketClient> _socketClients;

        public SocketServer(int port)
        {
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            _listenerSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));

            _socketClients = new ThreadSafeList<SocketClient>();
        }

        public void Start(int backlog = int.MaxValue)
        {
            if (_isStarted)
            {
                throw new SocketServerAlreadyStartedException();
            }
            
            _listenerSocket.Listen(backlog);
            
            Accept();

            _isStarted = true;
        }

        public void Stop()
        {
            if (!_isStarted)
            {
                throw new SocketServerAlreadyStoppedException();
            }
            
            _listenerSocket.Dispose();
            _listenerSocket = null;

            _isStarted = false;
        }
        
        private async void Accept()
        {
            do
            {
                try
                {
                    // Accept socket
                    var socket = await _listenerSocket.AcceptAsync();

                    // Wrap socket
                    var socketClient = new SocketClient(socket);

                    socketClient.DataReceived += SocketClientOnDataReceived;
                    socketClient.Disconnected += SocketClientOnDisconnected;

                    // Call socket connected event
                    SocketClientOnConnected(socketClient);
                    
                    // Start socket
                    socketClient.Start();
                }
                catch (SocketException socketException)
                {
                    if (!_isStarted)
                    {
                        // NOTE: Ignore. This exception probably raised because user stopped the server.
                        break;
                    }

                    // Rethrow
                    throw;
                }
            } while (_isStarted);
        }

        #region Socket Events

        private void SocketClientOnConnected(SocketClient socketClient)
        {
            _socketClients.Add(socketClient);
            
            OnConnected(socketClient.Id);
        }

        private void SocketClientOnDataReceived(SocketClient socketClient, byte[] data)
        {
            OnDataReceived(socketClient.Id, data);
        }
        
        private void SocketClientOnDisconnected(SocketClient socketClient, SocketClientDisconnectType disconnectType)
        {
            _socketClients.Remove(socketClient);
            
            OnDisconnected(socketClient.Id, disconnectType);
        }
        
        #endregion

        public async Task Send(SocketClientId id, byte[] buffer)
        {
            var socketClient = _socketClients.Find(i => i.Id == id);
                
            if (socketClient == null)
            {
                throw new SocketServerClientNotFoundException(id);
            }

            await socketClient.Send(buffer);
        }

        public void Disconnect(SocketClientId id)
        {
            var socketClient = _socketClients.Find(i => i.Id == id);
                
            if (socketClient == null)
            {
                throw new SocketServerClientNotFoundException(id);
            }
            
            socketClient.Disconnect();
        }
    }
}
