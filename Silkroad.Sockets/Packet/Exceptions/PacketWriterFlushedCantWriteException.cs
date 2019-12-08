using System;

namespace Silkroad.Sockets.Packet.Exceptions
{
    public class PacketWriterFlushedCantWriteException: Exception
    {
        public PacketWriterFlushedCantWriteException()
            : base("Can't write because PacketWriter is flushed.")
        {
            
        }
    }
}
