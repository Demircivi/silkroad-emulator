using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Silkroad.ConsoleExtensions;
using Silkroad.Sockets;
using Silkroad.Sockets.Abstract.Client.Enums;
using Silkroad.Sockets.Abstract.Client.Models;
using Silkroad.Sockets.Abstract.Server;
using Silkroad.Sockets.Packet;
using Silkroad.Sockets.Packet.Classic;

namespace Silkroad.Servers.Login
{
    internal static class Program
    {
        private const int Port = 15779;
        private static SilkroadSocketServer _server;

        private static void Main(string[] args)
        {
            Console.WriteLine($"Starting to listen on {Port}.");

            _server = new SilkroadSocketServer(Port);

            _server.Connected += ServerOnConnected;
            _server.DataReceived += ServerOnDataReceived;
            _server.Disconnected += ServerOnDisconnected;

            _server.Start();

            ConsoleRider.Start();
        }

        private static void ServerOnConnected(SocketClientId id)
        {
            Console.WriteLine($"User connected with id: {id}");

            var packetWriter = new PacketWriter(0x5000);

            packetWriter.WriteBoolean(true);

            _server.Send(id, packetWriter);
        }

        private static void ServerOnDataReceived(SocketClientId id, PacketReader packetReader)
        {
            Console.WriteLine($"User sent data: {BitConverter.ToString(packetReader.GetBuffer())} with id: {id}");

            if (packetReader.Opcode == 0x2001)
            {
                var packetWriter = new PacketWriter(0x2001);

                packetWriter.WriteAscii("GatewayServer");
                packetWriter.WriteBoolean(true);

                _server.Send(id, packetWriter);
            }
            else if (packetReader.Opcode == 0x6100)
            {
                var packetWriter = new PacketWriter(0xa100, true);

                packetWriter.WriteUInt8(1);

                _server.Send(id, packetWriter);
            }
            else if (packetReader.Opcode == 0x6104)
            {
                var items = new List<(string title, string content)>
                {
                    ("welcome to kral emulator", "silkroad scene is shit<br>welcome the <font color = red>kynq</font>"),
                    ("3", "4")
                };

                var packetWriter = new PacketWriter(0xa104, true);
                packetWriter.WriteUInt8((byte) items.Count);
                
                foreach (var item in items)
                {
                    packetWriter.WriteAscii(item.title);
                    packetWriter.WriteAscii(item.content);
                    packetWriter.WriteUInt16(2019);
                    packetWriter.WriteUInt16(12);
                    packetWriter.WriteUInt16(8);
                    packetWriter.WriteUInt16(21);
                    packetWriter.WriteUInt16(22);
                    packetWriter.WriteUInt16(0);
                    packetWriter.WriteUInt32(0);
                }

                _server.Send(id, packetWriter);
            }
        }

        private static void ServerOnDisconnected(SocketClientId id, SocketClientDisconnectType disconnectType)
        {
            Console.WriteLine($"User disconnected with id: {id}, disconnectType: {disconnectType}");
        }
    }
}
