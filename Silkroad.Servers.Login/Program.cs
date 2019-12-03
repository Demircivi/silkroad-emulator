using System;
using Silkroad.ConsoleExtensions;
using Silkroad.Sockets.Client.Models;
using Silkroad.Sockets.Server;

namespace Silkroad.Servers.Login
{
    internal static class Program
    {
        private const int Port = 15779;
        private static SocketServer _server;

        private static void Main(string[] args)
        {
            Console.WriteLine($"Starting to listen on {Port}.");

            _server = new SocketServer(Port);

            _server.Start();

            _server.Connected += ServerOnConnected;
            _server.DataReceived += ServerOnDataReceived;
            _server.Disconnected += ServerOnDisconnected;

            ConsoleRider.Start();
        }

        private static void ServerOnConnected(SocketClientId id)
        {
            Console.WriteLine($"User connected with id: {id}");

            _server.Send(id, new byte[] {1,0,0, 0x50, 0, 0, 1});
        }

        private static void ServerOnDataReceived(SocketClientId id, byte[] data)
        {
            Console.WriteLine($"User sent data: {BitConverter.ToString(data)} with id: {id}");
        }

        private static void ServerOnDisconnected(SocketClientId id)
        {
            Console.WriteLine($"User disconnected with id: {id}");
        }
    }
}
