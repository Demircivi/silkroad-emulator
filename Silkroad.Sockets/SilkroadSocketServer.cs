using System;
using System.IO;
using Silkroad.Sockets.Abstract.Client.Enums;
using Silkroad.Sockets.Abstract.Client.Models;
using Silkroad.Sockets.Abstract.Server;
using Silkroad.Sockets.Packet;

namespace Silkroad.Sockets
{
    public sealed class SilkroadSocketServer
    {
        #region Events

        public delegate void ConnectedDelegate(SocketClientId id);
        public delegate void DataReceivedDelegate(SocketClientId id, PacketReader packetReader);
        public delegate void DisconnectedDelegate(SocketClientId id, SocketClientDisconnectType disconnectType);
        
        public event ConnectedDelegate Connected;
        public event DataReceivedDelegate DataReceived;
        public event DisconnectedDelegate Disconnected;

        private void OnConnected(SocketClientId id)
        {
            Connected?.Invoke(id);
        }

        private void OnDataReceived(SocketClientId id, PacketReader packetReader)
        {
            DataReceived?.Invoke(id, packetReader);
        }

        private void OnDisconnected(SocketClientId id, SocketClientDisconnectType disconnectType)
        {
            Disconnected?.Invoke(id, disconnectType);
        }
        
        #endregion

        private readonly SocketServer _socketServer;
        
        public SilkroadSocketServer(int port)
        {
            _socketServer = new SocketServer(port);
            
            _socketServer.Connected += SocketServerOnConnected;
            _socketServer.DataReceived += SocketServerOnDataReceived;
            _socketServer.Disconnected += SocketServerOnDisconnected;
        }

        public void Start(int backlog = int.MaxValue)
        {
            _socketServer.Start(backlog);
        }

        public void Stop()
        {
            _socketServer.Stop();
        }

        public async void Send(SocketClientId id, PacketWriter packetWriter)
        {
            var bytes = packetWriter.GetBytes();
            Console.WriteLine(BitConverter.ToString(bytes));
            await _socketServer.Send(id, bytes);
        }
        
        public void Disconnect(SocketClientId id)
        {
            _socketServer.Disconnect(id);
        }
        
        #region Socket server event handlers
        
        private void SocketServerOnConnected(SocketClientId id)
        {
            OnConnected(id);
        }

        private void SocketServerOnDataReceived(SocketClientId id, byte[] data)
        {
            var memoryStream = new MemoryStream(data);
            var binaryReader = new BinaryReader(memoryStream);

            while (memoryStream.Position < memoryStream.Length)
            {
                var length = binaryReader.ReadUInt16() + 6;
                memoryStream.Seek(-2, SeekOrigin.Current);
                
                var packetBytes = binaryReader.ReadBytes(length);
                
                OnDataReceived(id, new PacketReader(packetBytes));
            }
        }

        private void SocketServerOnDisconnected(SocketClientId id, SocketClientDisconnectType disconnectType)
        {
            OnDisconnected(id, disconnectType);
        }
        
        #endregion
    }
}
