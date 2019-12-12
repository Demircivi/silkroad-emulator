using System;

namespace Silkroad.Sockets.Packet.Classic.Exceptions
{
    public class PacketWriterFlushedCantWriteException: Exception
    {
        public PacketWriterFlushedCantWriteException()
            : base("Can't write because PacketWriter is flushed.")
        {
            
        }
    }
}
